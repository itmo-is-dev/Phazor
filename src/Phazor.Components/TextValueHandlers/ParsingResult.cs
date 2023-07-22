namespace Phazor.Components.TextValueHandlers;

public record ParsingResult<T>
{
    private ParsingResult() { }

    public sealed record Success(T Value) : ParsingResult<T>;

    public sealed record Failure(string ErrorMessage) : ParsingResult<T>;
}