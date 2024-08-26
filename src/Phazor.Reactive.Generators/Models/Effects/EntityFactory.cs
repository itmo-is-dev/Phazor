using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Phazor.Reactive.Generators.Extensions;
using Phazor.Reactive.Generators.Models.Entities;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Phazor.Reactive.Generators.Models.Effects;

public record EntityFactory(ReactiveEntity Entity)
{
    public BackingField Field { get; } = BackingField.ForEntityFactory(Entity);

    public string Name { get; } = $"{Entity.Name}Factory";
    public string FullyQualifiedName { get; } = $"{Entity.FullyQualifiedName}Factory";

    public ClassDeclarationSyntax ToSyntax()
    {
        var entityInterfaceType = IdentifierName(Entity.InterfaceType.GetFullyQualifiedName());
        var identifierType = IdentifierName(Entity.IdentifierType.GetFullyQualifiedName());

        var baseType = GenericName(Constants.ReactiveFactoryBaseIdentifier)
            .AddTypeArgumentListArguments(entityInterfaceType)
            .AddTypeArgumentListArguments(identifierType);

        var objectCreation = ObjectCreationExpression(IdentifierName(Entity.Name))
            .AddArgumentListArguments(Argument(IdentifierName("id")));

        var createMethod = MethodDeclaration(entityInterfaceType, "CreateInternal")
            .AddModifiers(Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.OverrideKeyword))
            .AddParameterListParameters(Parameter(Identifier("id")).WithType(identifierType))
            .AddBodyStatements(ReturnStatement(objectCreation));

        return ClassDeclaration(Name)
            .AddModifiers(Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.SealedKeyword))
            .AddBaseListTypes(SimpleBaseType(baseType))
            .AddMembers(createMethod);
    }
}