using Phazor.Reactive.CollectionModifiers;
using System.Reactive.Subjects;

namespace Phazor.Reactive;

public class ReactiveCollectionProperty<T> : IReactiveCollectionProperty<T>
{
    private readonly HashSet<T> _values = [];
    private readonly ReplaySubject<IEnumerable<T>> _subject = new(1);
    private readonly ICollectionModifier<T> _collectionModifier = CollectionModifierFactory.Create<T>();

    public IDisposable Subscribe(IObserver<IEnumerable<T>> observer)
        => _subject.Subscribe(observer);

    public void Dispose()
        => _subject.Dispose();

    public void Add(T value)
    {
        _values.Add(value);
        Publish();
    }

    public void Add(IEnumerable<T> values)
    {
        foreach (T value in values)
        {
            _values.Add(value);
        }

        Publish();
    }

    public void Remove(T value)
    {
        _values.Remove(value);
        Publish();
    }

    public void Remove(IEnumerable<T> values)
    {
        foreach (T value in values)
            _values.Remove(value);

        Publish();
    }

    public void ReplaceBy(T value)
    {
        _values.Clear();
        _values.Add(value);

        Publish();
    }

    public void ReplaceBy(IEnumerable<T> values)
    {
        _values.Clear();

        foreach (T value in values)
        {
            _values.Add(value);
        }

        Publish();
    }

    public void Clear()
    {
        _values.Clear();
        Publish();
    }

    private void Publish()
    {
        _subject.OnNext(_collectionModifier.Apply(_values));
    }
}
