using Phazor.Reactive.Abstractions;
using Phazor.Reactive.CollectionModifiers;
using System.Reactive.Subjects;

namespace Phazor.Reactive;

public class ReactiveEntityCollectionProperty<TId, TEntity> : IReactiveCollectionProperty<TEntity>
    where TEntity : IReactiveEntity<TId>
{
    private readonly HashSet<TEntity> _values = [];
    private readonly ReplaySubject<IEnumerable<TEntity>> _subject = new(1);
    private readonly ICollectionModifier<TEntity> _collectionModifier = CollectionModifierFactory.Create<TEntity>();

    public IDisposable Subscribe(IObserver<IEnumerable<TEntity>> observer)
        => _subject.Subscribe(observer);

    public void Dispose()
        => _subject.Dispose();

    public void Add(TEntity value)
    {
        _values.Add(value);
        Publish();
    }

    public void Add(IEnumerable<TEntity> values)
    {
        foreach (TEntity value in values)
            _values.Add(value);

        Publish();
    }

    public void Remove(TEntity value)
    {
        _values.Remove(value);
        Publish();
    }

    public void Remove(IEnumerable<TEntity> values)
    {
        foreach (TEntity value in values)
            _values.Remove(value);

        Publish();
    }

    public void ReplaceBy(TEntity value)
    {
        _values.Clear();
        _values.Add(value);

        Publish();
    }

    public void ReplaceBy(IEnumerable<TEntity> values)
    {
        _values.Clear();

        foreach (TEntity value in values)
            _values.Add(value);

        Publish();
    }

    public void Clear()
    {
        _values.Clear();
        Publish();
    }

    private void Publish()
    {
        _subject.OnNext(_collectionModifier.Apply(_values).ToArray());
    }
}