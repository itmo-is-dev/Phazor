using Phazor.Reactive.Abstractions;

namespace Phazor.Reactive;

public interface IPhazorReactiveConfigurator
{
    IPhazorReactiveConfigurator AddEntityFactory<TEntity, TIdentifier, TFactory>()
        where TEntity : IReactiveEntity<TIdentifier>
        where TFactory : class, IReactiveEntityFactory<TEntity, TIdentifier>;

    IPhazorReactiveConfigurator AddEventHandler<TEvent, THandler>()
        where TEvent : IReactiveEvent<TEvent>
        where THandler : class, IReactiveEventHandler<TEvent>;
}