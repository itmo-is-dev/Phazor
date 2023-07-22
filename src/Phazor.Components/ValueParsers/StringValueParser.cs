using Phazor.Components.TextValueHandlers;

namespace Phazor.Components.ValueParsers;

public class StringValueParser : IValueParser<string>
{
    public static StringValueParser Instance { get; } = new StringValueParser();
    
    public ValueTask<ParsingResult<string>> ParseAsync(string input)
    {
        ParsingResult<string> result = new ParsingResult<string>.Success(input);
        return ValueTask.FromResult(result);
    }
}