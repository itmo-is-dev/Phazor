using Microsoft.AspNetCore.Components;
using Phazor.Components.ValueParsers;

namespace Phazor.Components.TextValueHandlers;

public class TextValueHandler<T> : ITextValueHandler<T>
{
    private readonly IValueParser<T> _parser;
    private EventCallback<T> _callback;

    public TextValueHandler(IValueParser<T> parser, EventCallback<T> callback)
    {
        _parser = parser;
        _callback = callback;
    }

    public async ValueTask HandleValueChangedAsync(string input)
    {
        ParsingResult<T> result = await _parser.ParseAsync(input);

        if (result is ParsingResult<T>.Success success)
        {
            await _callback.InvokeAsync(success.Value);
        }
    }

    public void Dispose()
    {
        _callback = default;
    }
}