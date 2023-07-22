using Microsoft.AspNetCore.Components;
using Phazor.Components.Tools;
using Disposable = System.Reactive.Disposables.Disposable;

namespace Phazor.Components;

public abstract class PhazorComponent : ComponentBase, IDisposable
{
    private IDisposable? _disposable;

    [Parameter]
    public string? ElementId { get; set; }

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string? Style { get; set; }

    protected virtual bool ShouldGenerateId => false;

    protected override void OnInitialized()
    {
        if (ShouldGenerateId)
        {
            ElementId = IdentifierGenerator.Next();
        }
    }

    protected override void OnParametersSet()
    {
        _disposable?.Dispose();
        _disposable = GenerateDisposable();
    }

    protected virtual IDisposable GenerateDisposable()
    {
        return Disposable.Empty;
    }

    public void Dispose()
    {
        _disposable?.Dispose();

        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) { }
}