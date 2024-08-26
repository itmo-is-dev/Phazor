using Microsoft.CodeAnalysis;
using Phazor.Reactive.Generators.Models.Entities;

namespace Phazor.Reactive.Generators.Models;

public record BackingField(string Name)
{
    public static BackingField ForReactiveProperty(IPropertySymbol property)
        => new($"_{char.ToLower(property.Name[0])}{property.Name[1..]}Subject");

    public static BackingField ForEntityFactory(ReactiveEntity entity)
        => new BackingField($"_{char.ToLower(entity.Name[0])}{entity.Name[1..]}Factory");
}