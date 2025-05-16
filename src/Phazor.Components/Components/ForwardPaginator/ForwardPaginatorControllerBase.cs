using System.Reactive;
using System.Reactive.Subjects;

namespace Phazor.Components;

public abstract class ForwardPaginatorControllerBase<TElement, TState> :
    IForwardPaginatorController<TElement, TState>,
    IDisposable
{
    private readonly Subject<Unit> _changedSubject = new();

    public IObservable<Unit> Changed => _changedSubject;

    public abstract TState CreateState();

    public abstract bool ShouldLoadNextPage(TState state);

    public abstract Task<ForwardPageLoadResult<TElement, TState>> LoadPageAsync(
        TState state,
        CancellationToken cancellationToken);

    protected void OnChanged()
    {
        _changedSubject.OnNext(Unit.Default);
    }

    public void Dispose()
    {
        _changedSubject.Dispose();
    }
}
