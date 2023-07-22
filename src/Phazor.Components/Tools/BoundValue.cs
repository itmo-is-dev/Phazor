using System.Reactive.Subjects;

namespace Phazor.Components.Tools;

public readonly struct BoundValue<T> : IObservable<T>, IEquatable<BoundValue<T>>
{
    private readonly IObservable<T>? _observable;

    internal BoundValue(IObservable<T> observable)
    {
        _observable = observable;
    }

    public static implicit operator BoundValue<T>(T value)
    {
        var subject = new ReplaySubject<T>(1);
        subject.OnNext(value);

        return new BoundValue<T>(subject);
    }

    public IDisposable Subscribe(IObserver<T> observer)
    {
        return _observable is null ? Disposable.Empty : _observable.Subscribe(observer);
    }

    public bool Equals(BoundValue<T> other)
        => Equals(_observable, other._observable);

    public override bool Equals(object? obj)
        => obj is BoundValue<T> other && Equals(other);

    public override int GetHashCode()
        => _observable?.GetHashCode() ?? 0;
}