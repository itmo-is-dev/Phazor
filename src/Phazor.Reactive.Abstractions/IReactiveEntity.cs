namespace Phazor.Reactive.Abstractions;

public interface IReactiveEntity<out TId> : IDisposable
{
    TId Id { get; }
}