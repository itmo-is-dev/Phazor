namespace Phazor.Components;

public record ForwardPageLoadResult<TElement, TState>(IEnumerable<TElement> Elements, TState State);