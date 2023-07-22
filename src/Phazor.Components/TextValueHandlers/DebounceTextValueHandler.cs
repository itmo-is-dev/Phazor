namespace Phazor.Components.TextValueHandlers;

public class DebounceTextValueHandler<T> : ITextValueHandler<T>
{
    private readonly ITextValueHandler<T> _sink;
    private readonly TimeSpan _delay;

    private CancellationTokenSource? _cts;

    public DebounceTextValueHandler(ITextValueHandler<T> sink, TimeSpan delay)
    {
        _sink = sink;
        _delay = delay;
    }

    public ValueTask HandleValueChangedAsync(string input)
    {
        _cts?.Cancel();
        _cts?.Dispose();

        _cts = new CancellationTokenSource();
        _ = SendInputAsync(input, _cts.Token);

        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        _cts?.Dispose();
    }

    private async Task SendInputAsync(string input, CancellationToken cancellationToken)
    {
        await Task.Delay(_delay, default);

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        await _sink.HandleValueChangedAsync(input);
    }
}