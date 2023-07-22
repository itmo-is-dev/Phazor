using Phazor.Components.Tools;

namespace Phazor.Components.Extensions;

public static class ObservableExtensions
{
    public static BoundValue<T> Bind<T>(this IObservable<T> observable)
    {
        return new BoundValue<T>(observable);
    }

    public static BoundValue<IEnumerable<T>> BindEnumerable<T>(IObservable<IEnumerable<T>> observable)
    {
        return new BoundValue<IEnumerable<T>>(observable);
    }
}