using Microsoft.Extensions.Options;
using Phazor.Reactive.Abstractions;

namespace Phazor.Reactive;

public interface IPhazorReactiveConfigurator
{
    IPhazorReactiveConfigurator AddEntityFactory<TEntity, TIdentifier, TAlias, TFactory>()
        where TEntity : IReactiveEntity<TIdentifier>
        where TAlias : class, IReactiveEntityFactory<TEntity, TIdentifier>
        where TFactory : class, TAlias;

    IPhazorReactiveConfigurator AddEventHandler<TEvent, THandler>()
        where TEvent : IReactiveEvent<TEvent>
        where THandler : class, IReactiveEventHandler<TEvent>;

    IPhazorReactiveConfigurator AddEventHandler<TEvent, THandler>(Func<IServiceProvider, THandler> factory)
        where TEvent : IReactiveEvent<TEvent>
        where THandler : class, IReactiveEventHandler<TEvent>;

    IPhazorReactiveConfigurator ConfigureOptions(Action<OptionsBuilder<PhazorReactiveOptions>> configuration);
}