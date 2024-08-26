using System.Linq.Expressions;

namespace Phazor.Reactive.Abstractions;

public abstract class ReactiveEventCollectionPropertyEffect<TEvent, TElement>
    where TEvent : IReactiveEvent<TEvent>
{
    private ReactiveEventCollectionPropertyEffect() { }

    public abstract void ByAdding(Expression<Func<TEvent, TElement>> expression);

    public abstract void ByAdding(Expression<Func<TEvent, IEnumerable<TElement>>> expression);

    public abstract void ByRemoving(Expression<Func<TEvent, TElement>> expression);

    public abstract void ByRemoving(Expression<Func<TEvent, IEnumerable<TElement>>> expression);

    public abstract void ByReplacingWith(Expression<Func<TEvent, TElement>> expression);

    public abstract void ByReplacingWith(Expression<Func<TEvent, IEnumerable<TElement>>> expression);

    public abstract void ByClearing();
}