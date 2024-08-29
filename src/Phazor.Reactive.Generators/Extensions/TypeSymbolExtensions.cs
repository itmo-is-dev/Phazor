using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Phazor.Reactive.Generators.Extensions;

public static class TypeSymbolExtensions
{
    public static string GetFullyQualifiedName(this INamespaceOrTypeSymbol symbol)
        => $"{symbol.ContainingNamespace.ToDisplayString()}.{symbol.Name}";

    public static IEnumerable<IdentifierNameSyntax> ToTypeArgumentSyntax(this IEnumerable<ITypeSymbol> symbols)
        => symbols.Select(ToTypeArgumentSyntax);

    public static IdentifierNameSyntax ToTypeArgumentSyntax(this ITypeSymbol symbol)
        => IdentifierName(symbol.GetFullyQualifiedName());

    public static TypeSyntax ToNameSyntax(this INamespaceOrTypeSymbol symbol, bool fullyQualified = true)
    {
        IReadOnlyCollection<IdentifierNameSyntax> typeParameters = symbol switch
        {
            INamedTypeSymbol namedTypeSymbol => namedTypeSymbol.TypeArguments.ToTypeArgumentSyntax().ToArray(),
            _ => [],
        };

        string name = fullyQualified ? symbol.GetFullyQualifiedName() : symbol.Name;

        TypeSyntax type = typeParameters.Count is 0
            ? IdentifierName(name)
            : GenericName(Identifier(name), TypeArgumentList(SeparatedList<TypeSyntax>(typeParameters)));

        if (symbol is not INamedTypeSymbol namedSymbol)
            return type;

        bool shouldAnnotateReferenceType = namedSymbol is
        {
            IsReferenceType: true,
            NullableAnnotation: NullableAnnotation.Annotated,
        };

        bool shouldAnnotateValueType = namedSymbol is
        {
            IsValueType: true,
            ConstructedFrom.SpecialType: not SpecialType.System_Nullable_T,
            NullableAnnotation: NullableAnnotation.Annotated,
        };

        if (shouldAnnotateReferenceType || shouldAnnotateValueType)
        {
            type = NullableType(type);
        }

        return type;
    }
}