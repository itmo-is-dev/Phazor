using System.Linq.Expressions;

namespace Phazor.Reactive.Abstractions;

public abstract class ReactiveEventPropertyEffect<TEvent, TProperty>
    where TEvent : IReactiveEvent<TEvent>
{
    private ReactiveEventPropertyEffect() { }

    public abstract void ByChangingTo(Expression<Func<TEvent, TProperty>> expression);
}