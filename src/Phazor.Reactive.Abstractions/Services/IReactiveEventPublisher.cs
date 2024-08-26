namespace Phazor.Reactive.Abstractions;

public interface IReactiveEventPublisher
{
    ValueTask PublishAsync<TEvent>(TEvent evt, CancellationToken cancellationToken)
        where TEvent : IReactiveEvent<TEvent>;
}