using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Phazor.Reactive.Abstractions;
using Phazor.Reactive.Handling;

namespace Phazor.Reactive.Services;

internal class ReactiveEventPublisher : IReactiveEventPublisher
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<ReactiveEventPublisher>? _logger;
    private readonly PhazorReactiveOptions _options;

    public ReactiveEventPublisher(
        IServiceProvider provider,
        IOptions<PhazorReactiveOptions> options,
        ILogger<ReactiveEventPublisher>? logger = null)
    {
        _provider = provider;
        _logger = logger;
        _options = options.Value;
    }

    public async ValueTask PublishAsync<TEvent>(TEvent evt, CancellationToken cancellationToken)
        where TEvent : IReactiveEvent<TEvent>
    {
        if (_options.EnableTracing)
        {
            _logger?.LogInformation("Publishing event = {Event}", evt);
        }

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