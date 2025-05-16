using System.Reactive;
using System.Reactive.Linq;

namespace Phazor.Components;

public interface IForwardPaginatorController<TElement, TState>
{
    IObservable<Unit> Changed => Observable.Empty<Unit>();

    TState CreateState();

    bool ShouldLoadNextPage(TState state);

    Task<ForwardPageLoadResult<TElement, TState>> LoadPageAsync(TState state, CancellationToken cancellationToken);
}
