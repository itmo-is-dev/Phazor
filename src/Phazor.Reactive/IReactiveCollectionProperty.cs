namespace Phazor.Reactive;

internal interface IReactiveCollectionProperty<T> : IObservable<IEnumerable<T>>, IDisposable
{
    void Add(T value);

    void Add(IEnumerable<T> values);

    void Remove(T value);

    void Remove(IEnumerable<T> values);

    void ReplaceBy(T value);

    void ReplaceBy(IEnumerable<T> values);

    void Clear();
}
