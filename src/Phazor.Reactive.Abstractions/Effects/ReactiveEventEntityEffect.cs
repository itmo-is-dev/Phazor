using System.Linq.Expressions;

namespace Phazor.Reactive.Abstractions;

public abstract class ReactiveEventEntityEffect<TEvent, TEntity, TId>
    where TEvent : IReactiveEvent<TEvent>
    where TEntity : IReactiveEntity<TId>
{
    private ReactiveEventEntityEffect() { }

    public abstract ReactiveEventPropertyEffect<TEvent, TProperty> AndItsProperty<TProperty>(
        Expression<Func<TEntity, IObservable<TProperty>>> expression);

    public abstract ReactiveEventCollectionPropertyEffect<TEvent, TElement> AndItsProperty<TElement>(
        Expression<Func<TEntity, IObservable<IEnumerable<TElement>>>> expression);
}