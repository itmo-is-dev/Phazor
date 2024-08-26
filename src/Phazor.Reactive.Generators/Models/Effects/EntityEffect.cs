using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Phazor.Reactive.Generators.Extensions;
using Phazor.Reactive.Generators.Models.Effects.PropertyEffects;
using System.Diagnostics.CodeAnalysis;

namespace Phazor.Reactive.Generators.Models.Effects;

public record EntityEffect(
    INamedTypeSymbol EntityType,
    SimpleLambdaExpressionSyntax GetIdFromEventExpression,
    IEntityPropertyEffect PropertyEffect)
{
    public IEnumerable<StatementSyntax> ToStatementSyntax(EffectGenerationContext context)
    {
        return PropertyEffect.ToStatementSyntax(context, this);
    }

    public static IEnumerable<IInvocationOperation> FilterEffectRoots(
        IEnumerable<IOperation> operations,
        Compilation compilation)
    {
        INamedTypeSymbol? handlerBuilderSymbol = compilation
            .GetTypeByMetadataName(Constants.HandlerBuilderMetadataName);

        if (handlerBuilderSymbol is null)
            return [];

        return operations
            .OfType<IInvocationOperation>()
            .Where(x =>
                x.Instance is IParameterReferenceOperation { Type: INamedTypeSymbol parameter }
                && parameter.ConstructedFrom.EqualsDefault(handlerBuilderSymbol));
    }

    public static bool TryGetEntityType(
        IInvocationOperation effectRoot,
        [NotNullWhen(true)] out INamedTypeSymbol? entityType)
    {
        entityType = effectRoot.TargetMethod.TypeArguments.FirstOrDefault() as INamedTypeSymbol;
        return entityType is not null;
    }

    public static bool TryGetIdentifierSelectorLambda(
        IInvocationOperation identifierSelectorOperation,
        [NotNullWhen(true)] out SimpleLambdaExpressionSyntax? lambda)
    {
        if (identifierSelectorOperation.Arguments is
            [{ Syntax: ArgumentSyntax { Expression: SimpleLambdaExpressionSyntax idSelectorLambda } }])
        {
            lambda = idSelectorLambda;
            return true;
        }

        lambda = null;
        return false;
    }

    public static bool TryGetProperty(
        IInvocationOperation propertySelectorOperation,
        [NotNullWhen(true)] out IPropertyReferenceOperation? property)
    {
        property = null;

        if (propertySelectorOperation.Arguments is not [{ Value: { } propertyOperation }])
            return false;

        property = propertyOperation
            .Descendants()
            .OfType<IPropertyReferenceOperation>()
            .SingleOrDefault();

        return property is not null;
    }
}