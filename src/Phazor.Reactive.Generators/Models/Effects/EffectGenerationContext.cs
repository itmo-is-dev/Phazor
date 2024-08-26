using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Phazor.Reactive.Generators.Extensions;
using Phazor.Reactive.Generators.Models.Entities;
using Phazor.Reactive.Generators.Models.Events;
using Phazor.Reactive.Generators.Tools;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Phazor.Reactive.Generators.Models.Effects;

public class EffectGenerationContext(IEnumerable<ReactiveEntity> entities)
{
    private readonly Dictionary<INamedTypeSymbol, EntityFactory> _factories = [];

    private readonly IReadOnlyDictionary<INamedTypeSymbol, ReactiveEntity> _entities = entities.ToDictionary(
        entity => entity.InterfaceType,
        new GenericSymbolEqualityComparer<INamedTypeSymbol>(SymbolEqualityComparer.Default));

    public EntityFactory GetFactory(INamedTypeSymbol symbol)
    {
        if (_factories.TryGetValue(symbol, out EntityFactory? factory))
            return factory;

        factory = new EntityFactory(GetEntity(symbol));
        _factories[symbol] = factory;

        return factory;
    }

    public ReactiveEntity GetEntity(INamedTypeSymbol entityType) => _entities[entityType];

    public IEnumerable<MemberDeclarationSyntax> ToMemberSyntax(ReactiveEventHandler handler)
    {
        foreach (EntityFactory factory in _factories.Values)
        {
            VariableDeclaratorSyntax fieldDeclarator = VariableDeclarator(factory.Field.Name);
            VariableDeclarationSyntax fieldDeclaration = VariableDeclaration(GetFactoryType(factory)).AddVariables(fieldDeclarator);

            yield return FieldDeclaration(fieldDeclaration)
                .AddModifiers(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword));
        }

        ParameterSyntax[] parameters = _factories.Values
            .Select(x => Parameter(Identifier(x.Field.Name)).WithType(GetFactoryType(x)))
            .ToArray();

        StatementSyntax[] assignments = _factories.Values
            .Select(StatementSyntax (factory) => ExpressionStatement(AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    ThisExpression(),
                    IdentifierName(factory.Field.Name)),
                IdentifierName(factory.Field.Name))))
            .ToArray();

        yield return ConstructorDeclaration(handler.Name)
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddParameterListParameters(parameters)
            .AddBodyStatements(assignments);
    }

    private GenericNameSyntax GetFactoryType(EntityFactory factory)
    {
        string identifierType = GetEntity(factory.Entity.InterfaceType).IdentifierType.GetFullyQualifiedName();

        return GenericName(Constants.EntityFactoryIdentifier)
            .AddTypeArgumentListArguments(IdentifierName(factory.Entity.InterfaceType.GetFullyQualifiedName()))
            .AddTypeArgumentListArguments(IdentifierName(identifierType));
    }
}