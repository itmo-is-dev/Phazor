using System.Linq.Expressions;

// ReSharper disable once CheckNamespace
namespace Phazor.Reactive.Abstractions;

public abstract class ReactiveEventEntitySelector<TEvent, TEntity, TId>
    where TEvent : IReactiveEvent<TEvent>
    where TEntity : IReactiveEntity<TId>
{
    private ReactiveEventEntitySelector() { }

    public abstract ReactiveEventEntityEffect<TEvent, TEntity, TId> SelectedBy(
        Expression<Func<TEvent, TId>> expression);
}