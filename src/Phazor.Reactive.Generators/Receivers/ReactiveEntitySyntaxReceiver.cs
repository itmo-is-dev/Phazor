using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Phazor.Reactive.Generators.Extensions;
using Phazor.Reactive.Generators.Models.Effects;
using Phazor.Reactive.Generators.Models.Effects.PropertyEffects;
using Phazor.Reactive.Generators.Models.Entities;
using Phazor.Reactive.Generators.Models.Entities.Properties;
using Phazor.Reactive.Generators.Models.Events;
using System.Diagnostics.CodeAnalysis;

namespace Phazor.Reactive.Generators.Receivers;

public class ReactiveEntitySyntaxReceiver : ISyntaxContextReceiver
{
    private readonly List<ReactiveEntity> _entities = [];
    private readonly List<ReactiveEvent> _events = [];

    public IReadOnlyCollection<ReactiveEntity> Entities => _entities;
    public IReadOnlyCollection<ReactiveEvent> Events => _events;

    public SyntaxNode? Node { get; private set; }

    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        if (context.Node is InterfaceDeclarationSyntax interfaceSyntax)
        {
            HandleInterfaceDeclaration(interfaceSyntax, context);
        }
        else if (context.Node is TypeDeclarationSyntax typeSyntax)
        {
            Node = context.Node;
            HandleTypeDeclaration(typeSyntax, context);
        }
    }

    private void HandleInterfaceDeclaration(InterfaceDeclarationSyntax interfaceSyntax, GeneratorSyntaxContext context)
    {
        var reactiveEntityUntypedInterface = context.SemanticModel.Compilation
            .GetTypeByMetadataName(Constants.ReactiveEntityMetadataName);

        var interfaceUntyped = context.SemanticModel.GetDeclaredSymbol(interfaceSyntax);

        if (reactiveEntityUntypedInterface is null || interfaceUntyped is not INamedTypeSymbol interfaceSymbol)
            return;

        var reactiveEntityInterface = interfaceSymbol.AllInterfaces
            .SingleOrDefault(x => x.ConstructedFrom.EqualsDefault(reactiveEntityUntypedInterface));

        if (reactiveEntityInterface?.TypeArguments is not [INamedTypeSymbol identifierType])
            return;

        var observableGenericInterface = context.SemanticModel.Compilation
            .GetTypeByMetadataName(Constants.ObservableMetadataName);

        if (observableGenericInterface is null)
            return;

        var observableProperties = interfaceSymbol
            .GetMembers()
            .OfType<IPropertySymbol>()
            .Where(x =>
                x.Type is INamedTypeSymbol namedTypeSymbol
                && namedTypeSymbol.ConstructedFrom.EqualsDefault(observableGenericInterface));

        var reactiveProperties = ParseProperties(observableProperties, context).ToArray();

        _entities.Add(new ReactiveEntity(interfaceSymbol, identifierType, reactiveProperties));
    }

    private void HandleTypeDeclaration(TypeDeclarationSyntax typeSyntax, GeneratorSyntaxContext context)
    {
        var genericReactiveEventInterface = context.SemanticModel.Compilation
            .GetTypeByMetadataName(Constants.ReactiveEventMetadataName);

        var typeUntyped = context.SemanticModel.GetDeclaredSymbol(typeSyntax);

        if (genericReactiveEventInterface is null || typeUntyped is not INamedTypeSymbol eventTypeSymbol)
            return;

        var reactiveEventInterface = genericReactiveEventInterface.Construct(eventTypeSymbol);

        if (eventTypeSymbol.AllInterfaces.Contains(reactiveEventInterface) is false)
            return;

        var handleMethodSymbol = eventTypeSymbol
            .GetMembers()
            .OfType<IMethodSymbol>()
            .SingleOrDefault(x => x.Name.Equals("Handle", StringComparison.OrdinalIgnoreCase));

        if (handleMethodSymbol is null)
            return;

        var handleMethodSyntax = handleMethodSymbol.DeclaringSyntaxReferences.Single().GetSyntax();
        var handleMethodOperation = context.SemanticModel.GetOperation(handleMethodSyntax);

        if (handleMethodOperation is not IMethodBodyOperation handleMethodBodyOperation)
            return;

        var handlerBuilderSymbol = context.SemanticModel.Compilation
            .GetTypeByMetadataName(Constants.HandlerBuilderMetadataName);

        if (handlerBuilderSymbol is null)
            return;

        var effectRoots = EntityEffect.FilterEffectRoots(
            operations: handleMethodBodyOperation.Descendants(),
            context.SemanticModel.Compilation);

        var effects = GetEffectFromRoots(effectRoots, context).ToArray();
        _events.Add(new ReactiveEvent(eventTypeSymbol, effects));
    }

    private IEnumerable<EntityEffect> GetEffectFromRoots(
        IEnumerable<IInvocationOperation> roots,
        GeneratorSyntaxContext context)
    {
        foreach (IInvocationOperation root in roots)
        {
            if (TryGetEffectFromRoot(root, context, out var effect))
                yield return effect;
        }
    }

    private bool TryGetEffectFromRoot(
        IInvocationOperation effectRoot,
        GeneratorSyntaxContext context,
        [NotNullWhen(true)] out EntityEffect? effect)
    {
        effect = null;

        if (EntityEffect.TryGetEntityType(effectRoot, out var entityType) is false)
            return false;

        if (effectRoot.Parent is not IInvocationOperation idSelectorOperation)
            return false;

        if (EntityEffect.TryGetIdentifierSelectorLambda(idSelectorOperation, out var idSelectorLambda) is false)
            return false;

        if (idSelectorOperation.Parent is not IInvocationOperation propertySelectorOperation)
            return false;

        if (EntityEffect.TryGetProperty(propertySelectorOperation, out var propertyReferenceOperation) is false)
            return false;

        var enumerableGenericSymbol = context.SemanticModel.Compilation
            .GetTypeByMetadataName(Constants.EnumerableMetadataName);

        if (enumerableGenericSymbol is null)
            return false;

        if (propertyReferenceOperation.Property.Type is not INamedTypeSymbol propertyObservableType)
            return false;

        if (propertyObservableType.TypeArguments is not [INamedTypeSymbol propertyValueType])
            return false;

        IEntityPropertyEffect? propertyEffect;

        if (propertyValueType.ConstructedFrom.EqualsDefault(enumerableGenericSymbol))
        {
            TryParseCollectionPropertyEffect(
                context.SemanticModel.Compilation,
                propertySelectorOperation,
                propertyReferenceOperation,
                out propertyEffect);
        }
        else
        {
            TryParsePropertyEffect(propertySelectorOperation, propertyReferenceOperation, out propertyEffect);
        }

        if (propertyEffect is null)
            return false;

        effect = new EntityEffect(
            entityType,
            idSelectorLambda,
            propertyEffect);

        return true;
    }

    private void TryParsePropertyEffect(
        IInvocationOperation propertySelectorOperation,
        IPropertyReferenceOperation propertyReferenceOperation,
        [NotNullWhen(true)] out IEntityPropertyEffect? effect)
    {
        effect = null;

        if (propertySelectorOperation.Parent is not IInvocationOperation changeToOperation)
            return;

        if (changeToOperation.Arguments is not
            [{ Syntax: ArgumentSyntax { Expression: SimpleLambdaExpressionSyntax valueLambda } }])
        {
            return;
        }

        effect = new PropertyEffect(propertyReferenceOperation.Property, valueLambda);
    }

    private void TryParseCollectionPropertyEffect(
        Compilation compilation,
        IInvocationOperation propertySelectorOperation,
        IPropertyReferenceOperation propertyReferenceOperation,
        [NotNullWhen(true)] out IEntityPropertyEffect? effect)
    {
        effect = null;

        if (propertySelectorOperation.Parent is not IInvocationOperation actionOperation)
            return;

        if (CollectionPropertyEffect.TryGetEffectAction(actionOperation, compilation, out var action) is false)
            return;

        effect = new CollectionPropertyEffect(
            propertyReferenceOperation.Property,
            action);
    }

    private IEnumerable<IReactiveProperty> ParseProperties(
        IEnumerable<IPropertySymbol> properties,
        GeneratorSyntaxContext context)
    {
        var enumerableGenericInterface = context.SemanticModel.Compilation
            .GetTypeByMetadataName(Constants.EnumerableMetadataName);

        if (enumerableGenericInterface is null)
            yield break;

        foreach (IPropertySymbol property in properties)
        {
            var typeSymbolUntyped = ((INamedTypeSymbol)property.Type).TypeArguments.Single();

            if (typeSymbolUntyped is not INamedTypeSymbol typeSymbol)
                continue;

            if (typeSymbol.ConstructedFrom.EqualsDefault(enumerableGenericInterface))
            {
                if (typeSymbol.TypeArguments.Single() is not INamedTypeSymbol elementSymbol)
                    continue;

                yield return new ReactiveCollectionProperty(property, elementSymbol);
            }
            else
            {
                yield return new ReactiveProperty(property, typeSymbol);
            }
        }
    }
}