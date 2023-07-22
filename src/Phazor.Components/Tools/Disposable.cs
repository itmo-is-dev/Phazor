namespace Phazor.Components.Tools;

public static class Disposable
{
    public static IDisposable From(params IDisposable[] disposables)
    {
        return new CollectionDisposable(disposables);
    }

    public static IDisposable Empty { get; } = new NoOpDisposable();

    private record CollectionDisposable(IEnumerable<IDisposable> Disposables) : IDisposable
    {
        public void Dispose()
        {
            foreach (IDisposable disposable in Disposables)
            {
                disposable.Dispose();
            }
        }
    }

    private class NoOpDisposable : IDisposable
    {
        public void Dispose() { }
    }
}