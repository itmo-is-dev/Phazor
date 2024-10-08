using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Phazor.Reactive.Abstractions;
using Phazor.Reactive.Handling;

namespace Phazor.Reactive;

internal sealed class PhazorReactiveConfigurator : IPhazorReactiveConfigurator
{
    private readonly IServiceCollection _collection;

    public PhazorReactiveConfigurator(IServiceCollection collection)
    {
        _collection = collection;
    }

    public IPhazorReactiveConfigurator AddEntityFactory<TEntity, TIdentifier, TAlias, TFactory>()
        where TEntity : IReactiveEntity<TIdentifier>
        where TAlias : class, IReactiveEntityFactory<TEntity, TIdentifier>
        where TFactory : class, TAlias
    {
        _collection.TryAddSingleton<TFactory>();
        _collection.TryAddSingleton<TAlias>(provider => provider.GetRequiredService<TFactory>());

        _collection.TryAddSingleton<IReactiveEntityFactory<TEntity, TIdentifier>>(
            provider => provider.GetRequiredService<TFactory>());

        return this;
    }

    public IPhazorReactiveConfigurator AddEventHandler<TEvent, THandler>()
        where TEvent : IReactiveEvent<TEvent>
        where THandler : class, IReactiveEventHandler<TEvent>
    {
        var descriptor = ServiceDescriptor.Singleton<
            IUntypedEventHandler,
            ReactiveEventHandlerWrapper<TEvent, THandler>>();

        _collection.TryAddSingleton<THandler>();
        _collection.Add(descriptor);

        return this;
    }

    public IPhazorReactiveConfigurator AddEventHandler<TEvent, THandler>(Func<IServiceProvider, THandler> factory)
        where TEvent : IReactiveEvent<TEvent>
        where THandler : class, IReactiveEventHandler<TEvent>
    {
        var descriptor = ServiceDescriptor.Singleton<
            IUntypedEventHandler,
            ReactiveEventHandlerWrapper<TEvent, THandler>>(
            implementationFactory: p => new ReactiveEventHandlerWrapper<TEvent, THandler>(
                factory.Invoke(p),
                p.GetRequiredService<IOptions<PhazorReactiveOptions>>(),
                p.GetService<ILogger<ReactiveEventHandlerWrapper<TEvent, THandler>>>()));

        _collection.TryAddSingleton<THandler>();
        _collection.Add(descriptor);

        return this;
    }

    public IPhazorReactiveConfigurator ConfigureOptions(Action<OptionsBuilder<PhazorReactiveOptions>> configuration)
    {
        OptionsBuilder<PhazorReactiveOptions> builder = _collection.AddOptions<PhazorReactiveOptions>();
        configuration.Invoke(builder);

        return this;
    }
}