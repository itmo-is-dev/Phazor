namespace Phazor.Reactive.CollectionModifiers;

internal interface ICollectionModifier<T>
{
    IEnumerable<T> Apply(IEnumerable<T> enumerable);
}
