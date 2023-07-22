using Phazor.Components.TextValueHandlers;

namespace Phazor.Components.ValueParsers;

public interface IValueParser<T>
{
    ValueTask<ParsingResult<T>> ParseAsync(string input);
}