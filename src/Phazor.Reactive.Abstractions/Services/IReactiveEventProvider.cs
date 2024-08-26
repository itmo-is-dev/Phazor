namespace Phazor.Reactive.Abstractions;

public interface IReactiveEventProvider
{
    IObservable<TEvent> Observe<TEvent>()
        where TEvent : IReactiveEvent<TEvent>;
}