using System.Text;

namespace Phazor.Components.Tools;

public class CssClassBuilder
{
    private readonly AdaptiveWeakReference<StringBuilder> _builder = new(static () => new StringBuilder());

    public CssClassBuilder Add(string value)
    {
        _builder.Value.Append(value);
        _builder.Value.Append(' ');

        return this;
    }

    public CssClassBuilder AddWhen(bool condition, string value)
        => condition ? Add(value) : this;

    public CssClassBuilder AddWhenNotNull(string? value)
        => string.IsNullOrEmpty(value) ? this : Add(value);

    public CssClassBuilder Use(ICssClassDirector director)
        => director.Direct(this);

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
