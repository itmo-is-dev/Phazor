#pragma warning disable CA1045

namespace Phazor.Extensions;

public static class ObservableExtensions
{
    public static void Subscribe<T>(this IObservable<T> observable, ref IDisposable disposable, Action<T> onNext)
    {
        disposable = disposable.Combine(observable.Subscribe(onNext));
    }
}