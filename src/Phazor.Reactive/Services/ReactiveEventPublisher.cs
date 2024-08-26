using Phazor.Reactive.Abstractions;
using Phazor.Reactive.Handling;

namespace Phazor.Reactive.Services;

internal class ReactiveEventPublisher : IReactiveEventPublisher
{
    private readonly IReadOnlyCollection<IUntypedEventHandler> _handlers;

    public ReactiveEventPublisher(IEnumerable<IUntypedEventHandler> wrappers)
    {
        _handlers = wrappers.ToArray();
    }

    public async ValueTask PublishAsync<TEvent>(TEvent evt, CancellationToken cancellationToken)
        where TEvent : IReactiveEvent<TEvent>
    {
        foreach (IUntypedEventHandler handler in _handlers)
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