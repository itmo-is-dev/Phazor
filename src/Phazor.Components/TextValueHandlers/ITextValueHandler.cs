namespace Phazor.Components.TextValueHandlers;

public interface ITextValueHandler<T> : IDisposable
{
    ValueTask HandleValueChangedAsync(string input);
}