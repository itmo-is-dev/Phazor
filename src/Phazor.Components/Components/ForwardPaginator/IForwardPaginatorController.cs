namespace Phazor.Components;

public interface IForwardPaginatorController<TElement, TState>
{
    TState CreateState();

    bool ShouldLoadNextPage(TState state);

    Task<ForwardPageLoadResult<TElement, TState>> LoadPageAsync(TState state, CancellationToken cancellationToken);
}