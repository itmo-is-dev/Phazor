namespace Phazor.Reactive.CollectionModifiers;

internal static class CollectionModifierFactory
{
    public static ICollectionModifier<T> Create<T>()
    {
        Type elementType = typeof(T);
        Type comparableType = typeof(IComparable<>);

        return elementType.IsAssignableTo(comparableType.MakeGenericType(elementType))
            ? OrderedCollectionModifier<T>.Instance
            : NoneCollectionModifier<T>.Instance;
    }
}
