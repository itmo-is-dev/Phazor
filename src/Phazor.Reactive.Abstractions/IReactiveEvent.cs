namespace Phazor.Reactive.Abstractions;

public interface IReactiveEvent<TSelf>
    where TSelf : IReactiveEvent<TSelf>
{
    static abstract void Handle(ReactiveEventHandler<TSelf> handler);
}