using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Phazor.Reactive.Abstractions;
using Phazor.Reactive.Handling;
using Phazor.Reactive.Services;

namespace Phazor.Reactive;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPhazorReactive(
        this IServiceCollection collection,
        Action<IPhazorReactiveConfigurator>? action = null)
    {
        collection.AddSingleton<ReactiveEventProvider>();
        collection.AddSingleton<IReactiveEventProvider>(p => p.GetRequiredService<ReactiveEventProvider>());

        collection.TryAddEnumerable(ServiceDescriptor.Singleton<IUntypedEventHandler, ProviderEventHandler>());

        collection.AddSingleton<IReactiveEventPublisher, ReactiveEventPublisher>();

        collection.AddOptions<PhazorReactiveOptions>();

        var configurator = new PhazorReactiveConfigurator(collection);
        action?.Invoke(configurator);

        return collection;
    }
}