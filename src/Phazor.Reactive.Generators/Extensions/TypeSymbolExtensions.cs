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

    public static IEnumerable<INamedTypeSymbol> GetBaseTypes(this INamedTypeSymbol symbol)
    {
        return symbol.BaseType is null
            ? []
            : Enumerable.Repeat(symbol.BaseType, 1).Concat(symbol.BaseType.GetBaseTypes());
    }

    public static IEnumerable<INamedTypeSymbol> GetBaseTypesAndInterfaces(this INamedTypeSymbol symbol)
    {
        return symbol.GetBaseTypes().Concat(symbol.AllInterfaces);
    }

    public static IEnumerable<INamedTypeSymbol> FindAssignableTypesConstructedFrom(
        this ITypeSymbol type,
        INamedTypeSymbol baseType)
    {
        if (type is not INamedTypeSymbol namedTypeSymbol)
            return Enumerable.Empty<INamedTypeSymbol>();

        IEnumerable<INamedTypeSymbol> baseTypes = namedTypeSymbol.GetBaseTypesAndInterfaces();

        return baseTypes
            .Where(current =>
                current.ConstructedFrom.Equals(baseType, SymbolEqualityComparer.Default));
    }

    public static INamedTypeSymbol? FindAssignableTypeConstructedFrom(
        this ITypeSymbol type,
        INamedTypeSymbol baseType)
    {
        IEnumerable<INamedTypeSymbol> symbols = type.FindAssignableTypesConstructedFrom(baseType);

        return symbols
            .FirstOrDefault(current =>
                current.ConstructedFrom.Equals(baseType, SymbolEqualityComparer.Default));
    }
}