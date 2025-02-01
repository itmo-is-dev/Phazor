using System.Text;

namespace Phazor.Components.Tools;

public class CssStyleBuilder
{
    private readonly AdaptiveWeakReference<StringBuilder> _builder = new(static () => new StringBuilder());

    public CssStyleBuilder Add(string key, string value)
    {
        _builder.Value.Append(key);
        _builder.Value.Append(": ");
        _builder.Value.Append(value);
        _builder.Value.Append("; ");

        return this;
    }

    public CssStyleBuilder AddWhen(bool condition, string key, string value)
        => condition ? Add(key, value) : this;

    public CssStyleBuilder AddWhen(bool condition, string key, string valueTrue, string valueFalse)
        => condition ? Add(key, valueTrue) : Add(key, valueFalse);

    public CssStyleBuilder AddWhenValueNotNull(string key, string? value)
        => string.IsNullOrEmpty(value) ? this : Add(key, value);

    public CssStyleBuilder AddWhenNotNull(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return this;

        StringBuilder builder = _builder.Value;

        builder.Append(value);

        if (value[^1] is ';')
        {
            builder.Append(' ');
        }
        else if (value[^2..] is not "; ")
        {
            builder.Append("; ");
        }

        return this;
    }

    public CssStyleBuilder Use(ICssStyleDirector director)
    {
        return director.Direct(this);
    }

    public void Clear()
    {
        _builder.Value.Clear();
        _builder.Strengthen();
    }

    public void OnUnchanged()
        => _builder.Weaken();

    public string Build()
    {
        StringBuilder builder = _builder.Value;

        if (builder.Length is 0)
            return string.Empty;

        if (builder[^1] is ' ')
            builder.Remove(builder.Length - 1, 1);

        return builder.ToString();
    }
}
