using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Phazor.Reactive.Generators.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Phazor.Reactive.Generators.Models.Entities.Properties;

public record ReactiveProperty(IPropertySymbol Property, INamedTypeSymbol Type) : IReactiveProperty
{
    private static readonly GenericNameSyntax FieldTypeName = GenericName(Constants.ReactivePropertyIdentifier);
    private static readonly GenericNameSyntax ObservableTypeName = GenericName(Constants.ObservableIdentifier);

    public IPropertySymbol PropertySymbol { get; } = Property;
    public BackingField BackingField { get; } = BackingField.ForReactiveProperty(Property);

    public IEnumerable<MemberDeclarationSyntax> ToMemberSyntax()
    {
        var valueType = IdentifierName(Type.GetFullyQualifiedName());

        var fieldInitializer = EqualsValueClause(ImplicitObjectCreationExpression());
        var fieldDeclarator = VariableDeclarator(BackingField.Name).WithInitializer(fieldInitializer);
        var fieldType = FieldTypeName.AddTypeArgumentListArguments(valueType);
        var fieldDeclaration = VariableDeclaration(fieldType).AddVariables(fieldDeclarator);

        yield return FieldDeclaration(fieldDeclaration)
            .AddModifiers(Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.ReadOnlyKeyword));

        var propertyBody = ArrowExpressionClause(IdentifierName(BackingField.Name));
        var propertyType = ObservableTypeName.AddTypeArgumentListArguments(valueType);
        var propertyName = Identifier(Property.Name);

        yield return PropertyDeclaration(propertyType, propertyName)
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .WithExpressionBody(propertyBody)
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
    }
}