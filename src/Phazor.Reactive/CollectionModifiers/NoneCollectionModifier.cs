namespace Phazor.Reactive.CollectionModifiers;

internal sealed class NoneCollectionModifier<T> : ICollectionModifier<T>
{
    public static readonly ICollectionModifier<T> Instance = new NoneCollectionModifier<T>();

    private NoneCollectionModifier() { }

    IEnumerable<T> ICollectionModifier<T>.Apply(IEnumerable<T> enumerable) => enumerable;
}
