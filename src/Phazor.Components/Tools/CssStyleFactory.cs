namespace Phazor.Components.Tools;

public class CssStyleFactory
{
    private readonly Func<CssStyleBuilder, CssStyleBuilder> _func;
    private readonly CssStyleBuilder _builder;

    private string? _css;

    public CssStyleFactory(Func<CssStyleBuilder, CssStyleBuilder> func)
    {
        _func = func;
        _builder = new CssStyleBuilder();
    }

    public string Value => _css ??= _func.Invoke(_builder.Clear()).Build();

    public void Invalidate()
    {
        _css = null;
    }
}