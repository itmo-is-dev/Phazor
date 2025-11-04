using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Phazor.Reactive.Generators.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Phazor.Reactive.Generators.Models.Entities.Properties;

public sealed record ReactiveValueProperty(IPropertySymbol Property, INamedTypeSymbol Type) : IReactiveProperty
{
    public IPropertySymbol PropertySymbol { get; } = Property;

    public BackingField BackingField { get; } = BackingField.Empty();

    public IEnumerable<MemberDeclarationSyntax> ToMemberSyntax(GeneratorExecutionContext context)
    {
        TypeSyntax valueType = Type.ToNameSyntax();

        TypeSyntax propertyType = valueType;
        SyntaxToken propertyName = Identifier(Property.Name);

        AccessorDeclarationSyntax getAccessor = AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        AccessorDeclarationSyntax setAccessor = AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
            .AddModifiers(Token(SyntaxKind.InternalKeyword))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        yield return PropertyDeclaration(propertyType, propertyName)
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddAccessorListAccessors(getAccessor, setAccessor);
    }
}
