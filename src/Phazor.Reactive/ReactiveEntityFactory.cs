using Phazor.Reactive.Abstractions;

namespace Phazor.Reactive;

public abstract class ReactiveEntityFactory<TEntity, TIdentifier> : IReactiveEntityFactory<TEntity, TIdentifier>
    where TEntity : class, IReactiveEntity<TIdentifier>
    where TIdentifier : notnull
{
    private readonly Dictionary<TIdentifier, WeakReference<TEntity>> _entities = [];

    public TEntity Create(TIdentifier id)
    {
        if (_entities.TryGetValue(id, out var reference) && reference.TryGetTarget(out var entity))
            return entity;

        entity = CreateInternal(id);
        _entities[id] = new WeakReference<TEntity>(entity);

        return entity;
    }

    protected abstract TEntity CreateInternal(TIdentifier id);
}