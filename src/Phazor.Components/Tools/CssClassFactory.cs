namespace Phazor.Components.Tools;

public class CssClassFactory
{
    private readonly Func<CssClassBuilder, CssClassBuilder> _func;
    private readonly CssClassBuilder _builder;

    private string? _css;

    public CssClassFactory(Func<CssClassBuilder, CssClassBuilder> func)
    {
        _func = func;
        _builder = new CssClassBuilder();
    }

    public string Value => _css ??= _func.Invoke(_builder.Clear()).Build();

    public void Invalidate()
    {
        _css = null;
    }
}