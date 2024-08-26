using Microsoft.CodeAnalysis;

namespace Phazor.Reactive.Generators.Tools;

public class GenericSymbolEqualityComparer<T>(SymbolEqualityComparer comparer) : IEqualityComparer<T>
    where T : ISymbol
{
    public bool Equals(T x, T y) => comparer.Equals(x, y);
    public int GetHashCode(T obj) => comparer.GetHashCode(obj);
}