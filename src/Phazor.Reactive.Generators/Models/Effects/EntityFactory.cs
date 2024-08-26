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
        IdentifierNameSyntax entityInterfaceType = IdentifierName(Entity.InterfaceType.GetFullyQualifiedName());
        IdentifierNameSyntax identifierType = IdentifierName(Entity.IdentifierType.GetFullyQualifiedName());

        GenericNameSyntax baseType = GenericName(Constants.ReactiveFactoryBaseIdentifier)
            .AddTypeArgumentListArguments(entityInterfaceType)
            .AddTypeArgumentListArguments(identifierType);

        ObjectCreationExpressionSyntax objectCreation = ObjectCreationExpression(IdentifierName(Entity.Name))
            .AddArgumentListArguments(Argument(IdentifierName("id")));

        MethodDeclarationSyntax createMethod = MethodDeclaration(entityInterfaceType, "CreateInternal")
            .AddModifiers(Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.OverrideKeyword))
            .AddParameterListParameters(Parameter(Identifier("id")).WithType(identifierType))
            .AddBodyStatements(ReturnStatement(objectCreation));

        return ClassDeclaration(Name)
            .AddModifiers(Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.SealedKeyword))
            .AddBaseListTypes(SimpleBaseType(baseType))
            .AddMembers(createMethod);
    }
}