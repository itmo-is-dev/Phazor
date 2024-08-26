using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

    public IPhazorReactiveConfigurator AddEntityFactory<TEntity, TIdentifier, TFactory>()
        where TEntity : IReactiveEntity<TIdentifier>
        where TFactory : class, IReactiveEntityFactory<TEntity, TIdentifier>
    {
        _collection.AddSingleton<IReactiveEntityFactory<TEntity, TIdentifier>, TFactory>();
        return this;
    }

    public IPhazorReactiveConfigurator AddEventHandler<TEvent, THandler>()
        where TEvent : IReactiveEvent<TEvent>
        where THandler : class, IReactiveEventHandler<TEvent>
    {
        var descriptor = ServiceDescriptor.Singleton<
            IUntypedEventHandler,
            ReactiveEventHandlerWrapper<TEvent, THandler>>();

        _collection.AddSingleton<THandler>();
        _collection.TryAddEnumerable(descriptor);

        return this;
    }
}