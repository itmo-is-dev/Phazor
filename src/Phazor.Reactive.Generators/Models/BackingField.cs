using Microsoft.CodeAnalysis;
using Phazor.Reactive.Generators.Models.Entities;

namespace Phazor.Reactive.Generators.Models;

public record BackingField(string Name, bool IsDisposable)
{
    public static BackingField ForReactiveProperty(IPropertySymbol property)
        => new($"_{char.ToLower(property.Name[0])}{property.Name[1..]}Subject", true);

    public static BackingField ForEntityFactory(ReactiveEntity entity)
        => new BackingField($"_{char.ToLower(entity.Name[0])}{entity.Name[1..]}Factory", true);

    public static BackingField Empty()
        => new(string.Empty, false);
}
