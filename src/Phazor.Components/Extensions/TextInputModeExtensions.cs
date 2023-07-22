using Phazor.Components.Models;

namespace Phazor.Components.Extensions;

public static class TextInputModeExtensions
{
    public static string ToDisplayString(this TextInputMode textInputMode)
    {
        return textInputMode switch
        {
            TextInputMode.Text => "text",
            TextInputMode.Phone => "tel",
            TextInputMode.Url => "url",
            TextInputMode.Email => "email",
            TextInputMode.Numeric => "numeric",
            TextInputMode.Decimal => "decimal",
            TextInputMode.Search => "search",
            _ or TextInputMode.None => string.Empty,
        };
    }
}