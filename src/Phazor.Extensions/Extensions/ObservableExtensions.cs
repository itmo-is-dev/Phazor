using System.Reactive;
using System.Reactive.Linq;

namespace Phazor.Extensions;

public static class ObservableExtensions
{
    public static IObservable<Unit> EraseType<T>(this IObservable<T> observable)
        => observable.Select(_ => Unit.Default);
}
