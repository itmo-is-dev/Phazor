namespace Phazor.Reactive.Abstractions;

public interface IReactiveEventHandler<in TEvent>
    where TEvent : IReactiveEvent<TEvent>
{
    ValueTask HandleAsync(TEvent evt, CancellationToken cancellationToken);
}