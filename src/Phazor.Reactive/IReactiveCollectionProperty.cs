namespace Phazor.Reactive;

internal interface IReactiveCollectionProperty<T> : IObservable<IEnumerable<T>>, IDisposable
{
    public void Add(T value);

    public void Add(IEnumerable<T> values);

    public void Remove(T value);

    public void Remove(IEnumerable<T> values);

    public void ReplaceBy(T value);

    public void ReplaceBy(IEnumerable<T> values);

    public void Clear();
}