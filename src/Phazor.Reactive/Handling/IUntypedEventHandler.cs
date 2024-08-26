using Phazor.Reactive.Abstractions;

namespace Phazor.Reactive.Handling;

internal interface IUntypedEventHandler
{
    ValueTask TryHandleAsync<TEvent>(TEvent evt, CancellationToken cancellationToken)
        where TEvent : IReactiveEvent<TEvent>;
}