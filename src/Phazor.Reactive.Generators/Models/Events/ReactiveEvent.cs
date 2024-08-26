using Microsoft.CodeAnalysis;
using Phazor.Reactive.Generators.Extensions;
using Phazor.Reactive.Generators.Models.Effects;

namespace Phazor.Reactive.Generators.Models.Events;

public record ReactiveEvent(INamedTypeSymbol EventType, IReadOnlyCollection<EntityEffect> Effects)
{
    public string Name { get; } = EventType.Name;

    public string FullyQualifiedName { get; } = EventType.GetFullyQualifiedName();
}