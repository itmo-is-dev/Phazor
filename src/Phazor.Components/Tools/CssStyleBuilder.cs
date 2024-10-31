using System.Text;

namespace Phazor.Components.Tools;

public class CssStyleBuilder
{
    private readonly StringBuilder _builder;

    public CssStyleBuilder()
    {
        _builder = new StringBuilder();
    }

    public CssStyleBuilder Add(string key, string value)
    {
        _builder.Append(key);
        _builder.Append(": ");
        _builder.Append(value);
        _builder.Append("; ");

        return this;
    }

    public CssStyleBuilder AddWhen(bool condition, string key, string value)
    {
        return condition ? Add(key, value) : this;
    }

    public CssStyleBuilder AddWhen(bool condition, string key, string valueTrue, string valueFalse)
    {
        return condition ? Add(key, valueTrue) : Add(key, valueFalse);
    }

    public CssStyleBuilder AddWhenValueNotNull(string key, string? value)
    {
        return string.IsNullOrEmpty(value) ? this : Add(key, value);
    }

    public CssStyleBuilder AddWhenNotNull(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return this;

        _builder.Append(value);

        if (value[^1] is ';')
        {
            _builder.Append(' ');
        }
        else if (value[^2..] is not "; ")
        {
            _builder.Append("; ");
        }

        return this;
    }

    public CssStyleBuilder Use(ICssStyleDirector director)
    {
        return director.Direct(this);
    }

    public CssStyleBuilder Clear()
    {
        _builder.Clear();
        return this;
    }

    public string Build()
    {
        if (_builder[^1] is ' ')
            _builder.Remove(_builder.Length - 1, 1);

        return _builder.ToString();
    }
}