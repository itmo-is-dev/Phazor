using Microsoft.CodeAnalysis;

namespace Phazor.Reactive.Generators.Extensions;

public static class TypeSymbolExtensions
{
    public static string GetFullyQualifiedName(this ITypeSymbol symbol)
        => $"{symbol.ContainingNamespace.ToDisplayString()}.{symbol.Name}";
}