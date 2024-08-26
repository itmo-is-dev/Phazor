using Microsoft.CodeAnalysis;

namespace Phazor.Reactive.Generators.Extensions;

public static class SymbolExtensions
{
    public static bool EqualsDefault(this ISymbol symbol, ISymbol? other)
        => symbol.Equals(other, SymbolEqualityComparer.Default);
}