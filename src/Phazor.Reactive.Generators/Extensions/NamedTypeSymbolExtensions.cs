using Microsoft.CodeAnalysis;

namespace Phazor.Reactive.Generators.Extensions;

public static class NamedTypeSymbolExtensions
{
    public static string GetFullyQualifiedName(this INamedTypeSymbol symbol) 
        => $"{symbol.ContainingNamespace.ToDisplayString()}.{symbol.Name}";
}