using Phazor.Reactive.Abstractions;

namespace Phazor.Reactive.Handling;

internal class ReactiveEventHandlerWrapper<THandlerEvent, THandler> : IUntypedEventHandler
    where THandlerEvent : IReactiveEvent<THandlerEvent>
    where THandler : class, IReactiveEventHandler<THandlerEvent>
{
    private readonly THandler _handler;

    public ReactiveEventHandlerWrapper(THandler handler)
    {
        _handler = handler;
    }

    public ValueTask TryHandleAsync<TEvent>(TEvent evt, CancellationToken cancellationToken)
        where TEvent : IReactiveEvent<TEvent>
    {
        return evt is THandlerEvent typedEvent
            ? _handler.HandleAsync(typedEvent, cancellationToken)
            : ValueTask.CompletedTask;
    }
}