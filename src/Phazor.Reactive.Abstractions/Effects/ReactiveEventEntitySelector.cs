using System.Linq.Expressions;

namespace Phazor.Reactive.Abstractions;

public abstract class ReactiveEventEntitySelector<TEvent, TEntity, TId>
    where TEvent : IReactiveEvent<TEvent>
    where TEntity : IReactiveEntity<TId>
{
    private ReactiveEventEntitySelector() { }

    public abstract ReactiveEventEntityEffect<TEvent, TEntity, TId> SelectedBy(
        Expression<Func<TEvent, TId>> expression);
}