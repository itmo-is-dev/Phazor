using System.Text;

namespace Phazor.Components.Tools;

public class CssClassBuilder
{
    private readonly StringBuilder _builder = new();

    public CssClassBuilder Add(string value)
    {
        _builder.Append(value);
        _builder.Append(' ');

        return this;
    }

    public CssClassBuilder AddWhen(bool condition, string value)
    {
        return condition ? Add(value) : this;
    }

    public CssClassBuilder AddWhenNotNull(string? value)
    {
        return string.IsNullOrEmpty(value) ? this : Add(value);
    }

    public CssClassBuilder Use(ICssClassDirector director)
    {
        return director.Direct(this);
    }

    public CssClassBuilder Clear()
    {
        _builder.Clear();
        return this;
    }

    public string Build()
    {
        if (_builder.Length is 0)
            return string.Empty;

        if (_builder[^1] is ' ')
            _builder.Remove(_builder.Length - 1, 1);

        return _builder.ToString();
    }
}