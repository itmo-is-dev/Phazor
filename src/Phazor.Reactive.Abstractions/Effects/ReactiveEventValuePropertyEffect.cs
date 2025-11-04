using System.Linq.Expressions;

// ReSharper disable once CheckNamespace
namespace Phazor.Reactive.Abstractions;

public abstract class ReactiveEventValuePropertyEffect<TEvent, TProperty>
    where TEvent : IReactiveEvent<TEvent>
{
    private ReactiveEventValuePropertyEffect() { }

    public abstract void ByChangingTo(Expression<Func<TEvent, TProperty>> expression);
}
