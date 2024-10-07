namespace Phazor.Reactive.Abstractions;

public interface IReactiveEntity<TId> : IDisposable
{
    TId Id { get; }
}