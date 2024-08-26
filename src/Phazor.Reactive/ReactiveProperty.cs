using System.Reactive.Subjects;

namespace Phazor.Reactive;

public class ReactiveProperty<T> : IObservable<T>, IDisposable
{
    private readonly ReplaySubject<T> _subject = new(1);

    public IDisposable Subscribe(IObserver<T> observer)
        => _subject.Subscribe(observer);

    public void Dispose()
        => _subject.Dispose();

    public void ChangeTo(T value)
        => _subject.OnNext(value);
}