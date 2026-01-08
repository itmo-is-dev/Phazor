using System.Diagnostics.CodeAnalysis;

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace Phazor.Extensions;

public static class DisposableExtensions
{
    [return: NotNullIfNotNull(nameof(disposable))]
    public static IDisposable? Combine(this IDisposable? disposable, IDisposable? other)
    {
        return (disposable, other) switch
        {
            (null, null) => null,
            (not null, null) => disposable,
            (null, not null) => other,
            _ => new CombinedDisposable(disposable, other),
        };
    }

#pragma warning disable CA1045
    public static void CombineTo(this IDisposable disposable, ref IDisposable other)
    {
        other = other.Combine(disposable);
    }

    private class CombinedDisposable : IDisposable
    {
        private readonly IDisposable _first;
        private readonly IDisposable _second;

        public CombinedDisposable(IDisposable first, IDisposable second)
        {
            _first = first;
            _second = second;
        }

        public void Dispose()
        {
            _second.Dispose();
            _first.Dispose();
        }
    }
}
