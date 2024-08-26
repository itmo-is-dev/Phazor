namespace Phazor.Reactive.Abstractions;

public interface IReactiveEntityFactory<TEntity, TId>
    where TEntity : IReactiveEntity<TId>
{
    TEntity Create(TId id);
}