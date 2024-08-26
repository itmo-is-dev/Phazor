namespace Phazor.Reactive.Abstractions;

public abstract class ReactiveEventHandler<TEvent>
    where TEvent : IReactiveEvent<TEvent>
{
    private ReactiveEventHandler() { }

    public abstract ReactiveEventEntitySelector<TEvent, TEntity, TId> Affects<TEntity, TId>()
        where TEntity : IReactiveEntity<TId>;
}