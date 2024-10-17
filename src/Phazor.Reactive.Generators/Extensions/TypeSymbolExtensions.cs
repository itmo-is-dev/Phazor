using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics.CodeAnalysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Phazor.Reactive.Generators.Extensions;

public static class TypeSymbolExtensions
{
    private static SymbolDisplayFormat ConfigureDisplayFormat(SymbolDisplayFormat format) => format
        .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)
        .WithGenericsOptions(SymbolDisplayGenericsOptions.None);

    private static readonly SymbolDisplayFormat FullyQualifiedSymbolFormat =
        ConfigureDisplayFormat(SymbolDisplayFormat.FullyQualifiedFormat);

    private static readonly SymbolDisplayFormat ShortSymbolFormat =
        ConfigureDisplayFormat(SymbolDisplayFormat.MinimallyQualifiedFormat);

    private static bool TryGetTypeArgumentSyntax(
        this INamespaceOrTypeSymbol symbol,
        bool fullyQualified,
        [NotNullWhen(true)] out TypeArgumentListSyntax? syntax)
    {
        bool isNullableValueType = symbol is INamedTypeSymbol
        {
            ConstructedFrom.SpecialType: SpecialType.System_Nullable_T,
        };

        if (symbol is INamedTypeSymbol { TypeArguments: { Length: not 0 } typeArguments }
            && isNullableValueType is false)
        {
            syntax = TypeArgumentList(SeparatedList(typeArguments.Select(x => x.ToNameSyntax(fullyQualified))));
            return true;
        }

        syntax = null;
        return false;
    }

    public static string GetFullyQualifiedName(this INamespaceOrTypeSymbol symbol)
        => symbol.ToDisplayString(FullyQualifiedSymbolFormat);

    public static string GetShortName(this INamespaceOrTypeSymbol symbol)
        => symbol.ToDisplayString(ShortSymbolFormat);

    public static TypeSyntax ToNameSyntax(this INamespaceOrTypeSymbol symbol, bool fullyQualified = true)
    {
        string name = fullyQualified ? symbol.GetFullyQualifiedName() : symbol.GetShortName();

        TypeSyntax type = TryGetTypeArgumentSyntax(symbol, fullyQualified, out TypeArgumentListSyntax? typeArguments)
            ? GenericName(Identifier(name), typeArguments)
            : IdentifierName(name);
        
        bool shouldAnnotateReferenceType = symbol is INamedTypeSymbol
        {
            IsReferenceType: true,
            NullableAnnotation: NullableAnnotation.Annotated,
        };

        if (shouldAnnotateReferenceType)
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