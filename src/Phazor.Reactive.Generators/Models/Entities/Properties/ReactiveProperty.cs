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

    public IEnumerable<MemberDeclarationSyntax> ToMemberSyntax(GeneratorExecutionContext context)
    {
        TypeSyntax valueType = Type.ToNameSyntax();

        EqualsValueClauseSyntax fieldInitializer = EqualsValueClause(ImplicitObjectCreationExpression());

        VariableDeclaratorSyntax fieldDeclarator = VariableDeclarator(BackingField.Name)
            .WithInitializer(fieldInitializer);

        GenericNameSyntax fieldType = FieldTypeName.AddTypeArgumentListArguments(valueType);
        VariableDeclarationSyntax fieldDeclaration = VariableDeclaration(fieldType).AddVariables(fieldDeclarator);

        yield return FieldDeclaration(fieldDeclaration)
            .AddModifiers(Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.ReadOnlyKeyword));

        ArrowExpressionClauseSyntax propertyBody = ArrowExpressionClause(IdentifierName(BackingField.Name));
        GenericNameSyntax propertyType = ObservableTypeName.AddTypeArgumentListArguments(valueType);
        SyntaxToken propertyName = Identifier(Property.Name);

        yield return PropertyDeclaration(propertyType, propertyName)
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .WithExpressionBody(propertyBody)
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
    }
}
