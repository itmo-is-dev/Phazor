namespace Phazor.Reactive.CollectionModifiers;

internal sealed class OrderedCollectionModifier<T> : ICollectionModifier<T>
{
    public static readonly ICollectionModifier<T> Instance = new OrderedCollectionModifier<T>();

    private OrderedCollectionModifier() { }

    IEnumerable<T> ICollectionModifier<T>.Apply(IEnumerable<T> enumerable) => enumerable.Order();
}
