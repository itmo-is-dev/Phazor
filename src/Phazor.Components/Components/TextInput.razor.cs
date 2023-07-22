using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Phazor.Components.Extensions;
using Phazor.Components.Models;
using Phazor.Components.TextValueHandlers;
using Phazor.Components.Tools;
using Phazor.Components.ValueParsers;
using System.Reactive.Linq;

namespace Phazor.Components;

public partial class TextInput : PhazorComponent
{
    private readonly EventCallback<string> _callback;

    private ITextValueHandler<string>? _handler;
    private IDisposable? _valueSubscription;

    private string _value;

    public TextInput()
    {
        _callback = new EventCallback<string>(this, HandleCallbackAsync);
        _value = string.Empty;
    }

    [Parameter]
    public TextType Type { get; set; }

    [Parameter]
    public TextInputMode Mode { get; set; }

    [Parameter]
    public TimeSpan DebounceDelay { get; set; }

    [Parameter]
    public EventCallback<string> TextChanged { get; set; }

    [Parameter]
    public EventCallback<string> TextSubmitted { get; set; }

    [Parameter]
    public EventCallback<KeyboardEventArgs> KeyUp { get; set; }

    [Parameter]
    public bool ReadOnly { get; set; }

    [Parameter]
    public BoundValue<string> Value { get; set; }

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        if (parameters.TryGetUpdatedValue(nameof(DebounceDelay), DebounceDelay, out TimeSpan? debounceDelay))
        {
            _handler = new TextValueHandler<string>(StringValueParser.Instance, _callback);
            _handler = new DebounceTextValueHandler<string>(_handler, debounceDelay.Value);
        }

        if (parameters.TryGetUpdatedValue(nameof(Value), Value, out BoundValue<string>? value))
        {
            _valueSubscription?.Dispose();
            _valueSubscription = value.Value.Subscribe(x => _ = InvokeAsync(() => HandleCallbackAsync(x)));
        }

        await base.SetParametersAsync(parameters);
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (_handler is null)
        {
            _handler = new TextValueHandler<string>(StringValueParser.Instance, _callback);
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _handler?.Dispose();
            _valueSubscription?.Dispose();
        }
    }

    private async Task HandleCallbackAsync(string value)
    {
        _value = value;
        await TextChanged.InvokeAsync(value);
    }

    private async ValueTask OnInputAsync(ChangeEventArgs args)
    {
        if (ReadOnly)
            return;

        if (_handler is null)
        {
            return;
        }

        string input = args.Value?.ToString() ?? string.Empty;
        await _handler.HandleValueChangedAsync(input);
    }

    private async ValueTask OnKeyUpAsync(KeyboardEventArgs args)
    {
        if (args.Key is "Enter")
        {
            await TextSubmitted.InvokeAsync(_value);
        }

        await KeyUp.InvokeAsync(args);
    }
}