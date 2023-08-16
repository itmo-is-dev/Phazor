namespace Phazor.Extensions;

public static class DisposableExtensions
{
    public static IDisposable Combine(this IDisposable disposable, IDisposable other)
    {
        return new CombinedDisposable(disposable, other);
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