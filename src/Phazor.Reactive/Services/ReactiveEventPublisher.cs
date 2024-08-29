using Microsoft.Extensions.DependencyInjection;
using Phazor.Reactive.Abstractions;
using Phazor.Reactive.Handling;

namespace Phazor.Reactive.Services;

internal class ReactiveEventPublisher : IReactiveEventPublisher
{
    private readonly IServiceProvider _provider;

    public ReactiveEventPublisher(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async ValueTask PublishAsync<TEvent>(TEvent evt, CancellationToken cancellationToken)
        where TEvent : IReactiveEvent<TEvent>
    {
        foreach (IUntypedEventHandler handler in _provider.GetRequiredService<IEnumerable<IUntypedEventHandler>>())
        {
            await handler.TryHandleAsync(evt, cancellationToken);
        }
    }

    public async ValueTask PublishAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken)
        where TEvent : IReactiveEvent<TEvent>
    {
        foreach (TEvent evt in events)
        {
            await PublishAsync(evt, cancellationToken);
        }
    }
}