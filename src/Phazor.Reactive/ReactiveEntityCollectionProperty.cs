using Phazor.Reactive.Abstractions;
using System.Reactive.Subjects;

namespace Phazor.Reactive;

public class ReactiveEntityCollectionProperty<TId, TEntity> : IReactiveCollectionProperty<TEntity>
    where TEntity : IReactiveEntity<TId>
{
    private readonly HashSet<TEntity> _values = [];
    private readonly ReplaySubject<IEnumerable<TEntity>> _subject = new(1);

    public IDisposable Subscribe(IObserver<IEnumerable<TEntity>> observer)
        => _subject.Subscribe(observer);

    public void Dispose()
        => _subject.Dispose();

    public void Add(TEntity value)
    {
        _values.Add(value);
        _subject.OnNext(_values);
    }

    public void Add(IEnumerable<TEntity> values)
    {
        foreach (TEntity value in values)
            _values.Add(value);

        _subject.OnNext(_values);
    }

    public void Remove(TEntity value)
    {
        _values.Remove(value);
        _subject.OnNext(_values);
    }

    public void Remove(IEnumerable<TEntity> values)
    {
        foreach (TEntity value in values)
            _values.Remove(value);

        _subject.OnNext(_values);
    }

    public void ReplaceBy(TEntity value)
    {
        _values.Clear();
        _values.Add(value);

        _subject.OnNext(_values);
    }

    public void ReplaceBy(IEnumerable<TEntity> values)
    {
        _values.Clear();

        foreach (TEntity value in values)
            _values.Add(value);

        _subject.OnNext(_values);
    }

    public void Clear()
    {
        _values.Clear();
        _subject.OnNext(_values);
    }
}