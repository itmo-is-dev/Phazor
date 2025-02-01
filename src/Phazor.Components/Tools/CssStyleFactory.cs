namespace Phazor.Components.Tools;

public class CssStyleFactory
{
    private readonly Func<CssStyleBuilder, CssStyleBuilder> _func;
    private readonly CssStyleBuilder _builder;
    private readonly AdaptiveWeakReference<string> _css;

    public CssStyleFactory(Func<CssStyleBuilder, CssStyleBuilder> func)
    {
        _func = func;
        _builder = new CssStyleBuilder();
        _css = new AdaptiveWeakReference<string>(static () => string.Empty);
    }

    public string Value => GetValue();

    public void Invalidate()
    {
        _css.Clear();
        _css.Weaken();
    }

    private string GetValue()
    {
        if (_css.HasValue)
        {
            _css.Strengthen();
            _builder.OnUnchanged();

            return _css.Value;
        }

        _builder.Clear();
        _func.Invoke(_builder);

        string value = _builder.Build();
        _css.Value = value;

        return value;
    }
}
