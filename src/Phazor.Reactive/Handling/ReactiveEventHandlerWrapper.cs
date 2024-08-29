using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Phazor.Reactive.Abstractions;

namespace Phazor.Reactive.Handling;

internal class ReactiveEventHandlerWrapper<THandlerEvent, THandler> : IUntypedEventHandler
    where THandlerEvent : IReactiveEvent<THandlerEvent>
    where THandler : class, IReactiveEventHandler<THandlerEvent>
{
    private readonly THandler _handler;
    private readonly PhazorReactiveOptions _options;
    private readonly ILogger<ReactiveEventHandlerWrapper<THandlerEvent, THandler>>? _logger;

    public ReactiveEventHandlerWrapper(
        THandler handler,
        IOptions<PhazorReactiveOptions> options,
        ILogger<ReactiveEventHandlerWrapper<THandlerEvent, THandler>>? logger = null)
    {
        _handler = handler;
        _options = options.Value;
        _logger = logger;
    }

    public async ValueTask TryHandleAsync<TEvent>(TEvent evt, CancellationToken cancellationToken)
        where TEvent : IReactiveEvent<TEvent>
    {
        if (evt is not THandlerEvent typedEvent)
            return;

        if (_options.EnableTracing)
        {
            _logger?.LogTrace("Started handling of event = {Event}, by handler of type = {HandlerType}",
                typedEvent,
                typeof(THandler));
        }

        await _handler.HandleAsync(typedEvent, cancellationToken);
    }
}