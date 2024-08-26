using System.Reactive.Subjects;

namespace Phazor.Reactive;

public class ReactiveCollectionProperty<T> : IObservable<IEnumerable<T>>, IDisposable
{
    private readonly List<T> _values = [];
    private readonly ReplaySubject<IEnumerable<T>> _subject = new(1);

    public IDisposable Subscribe(IObserver<IEnumerable<T>> observer)
        => _subject.Subscribe(observer);

    public void Dispose()
        => _subject.Dispose();

    public void Add(T value)
    {
        _values.Add(value);
        _subject.OnNext(_values);
    }

    public void Add(IEnumerable<T> values)
    {
        _values.AddRange(values);
        _subject.OnNext(_values);
    }

    public void Remove(T value)
    {
        _values.Remove(value);
        _subject.OnNext(_values);
    }

    public void Remove(IEnumerable<T> values)
    {
        foreach (T? value in values)
            _values.Remove(value);

        _subject.OnNext(_values);
    }

    public void ReplaceBy(T value)
    {
        _values.Clear();
        _values.Add(value);

        _subject.OnNext(_values);
    }

    public void ReplaceBy(IEnumerable<T> values)
    {
        _values.Clear();
        _values.AddRange(values);

        _subject.OnNext(_values);
    }

    public void Clear()
    {
        _values.Clear();
        _subject.OnNext(_values);
    }
}