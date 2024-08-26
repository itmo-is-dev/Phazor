using Phazor.Reactive.Abstractions;
using Phazor.Reactive.Services;

namespace Phazor.Reactive.Handling;

internal class ProviderEventHandler : IUntypedEventHandler
{
    private readonly ReactiveEventProvider _provider;

    public ProviderEventHandler(ReactiveEventProvider provider)
    {
        _provider = provider;
    }

    public ValueTask TryHandleAsync<TEvent>(TEvent evt, CancellationToken cancellationToken)
        where TEvent : IReactiveEvent<TEvent>
    {
        _provider.Publish(evt);
        return ValueTask.CompletedTask;
    }
}