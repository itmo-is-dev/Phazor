using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Phazor.Reactive.Generators.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Phazor.Reactive.Generators.Models.Entities.Properties;

public record ReactiveCollectionProperty(IPropertySymbol Property, INamedTypeSymbol ElementType) : IReactiveProperty
{
    private static readonly GenericNameSyntax FieldTypeName =
        GenericName(Constants.ReactiveCollectionPropertyIdentifier);

    private static readonly GenericNameSyntax EntityFieldTypeName =
        GenericName(Constants.ReactiveEntityCollectionPropertyIdentifier);

    private static readonly GenericNameSyntax ObservableTypeName =
        GenericName(Constants.ObservableIdentifier);

    private static readonly GenericNameSyntax EnumerableTypeName =
        GenericName(Constants.EnumerableIdentifier);

    public IPropertySymbol PropertySymbol { get; } = Property;
    public BackingField BackingField { get; } = BackingField.ForReactiveProperty(Property);

    public IEnumerable<MemberDeclarationSyntax> ToMemberSyntax(GeneratorExecutionContext context)
    {
        TypeSyntax elementType = ElementType.ToNameSyntax();
        GenericNameSyntax enumerableType = EnumerableTypeName.AddTypeArgumentListArguments(elementType);

        EqualsValueClauseSyntax fieldInitializer = EqualsValueClause(ImplicitObjectCreationExpression());

        VariableDeclaratorSyntax fieldDeclarator = VariableDeclarator(BackingField.Name)
            .WithInitializer(fieldInitializer);

        GenericNameSyntax fieldType = GetFieldNameSyntax(context).AddTypeArgumentListArguments(elementType);
        VariableDeclarationSyntax fieldDeclaration = VariableDeclaration(fieldType).AddVariables(fieldDeclarator);

        yield return FieldDeclaration(fieldDeclaration)
            .AddModifiers(Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.ReadOnlyKeyword));

        ArrowExpressionClauseSyntax propertyBody = ArrowExpressionClause(IdentifierName(BackingField.Name));
        GenericNameSyntax propertyType = ObservableTypeName.AddTypeArgumentListArguments(enumerableType);
        SyntaxToken propertyName = Identifier(Property.Name);

        yield return PropertyDeclaration(propertyType, propertyName)
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .WithExpressionBody(propertyBody)
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
    }

    private GenericNameSyntax GetFieldNameSyntax(GeneratorExecutionContext context)
    {
        INamedTypeSymbol? entityType = context.Compilation.GetTypeByMetadataName(Constants.ReactiveEntityMetadataName);

        if (entityType is null)
            return FieldTypeName;

        INamedTypeSymbol? madeEntityType = ElementType.FindAssignableTypeConstructedFrom(entityType);

        if (madeEntityType is null)
            return FieldTypeName;

        ITypeSymbol identifierType = madeEntityType.TypeArguments.Single();
        return EntityFieldTypeName.AddTypeArgumentListArguments(identifierType.ToNameSyntax());
    }
}