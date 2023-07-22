using Phazor.Components.Models;

namespace Phazor.Components.Extensions;

public static class TextTypeExtensions
{
    public static string ToDisplayString(this TextType textRole)
    {
        return textRole switch
        {
            TextType.Email => "email",
            TextType.Password => "password",
            TextType.Url => "url",
            TextType.Search => "search",
            TextType.Telephone => "tel",
            _ or TextType.Text => "text",
        };
    }
}