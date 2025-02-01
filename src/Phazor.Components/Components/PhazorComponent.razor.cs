using Microsoft.AspNetCore.Components;
using Phazor.Components.Extensions;
using Phazor.Components.Models;
using Phazor.Components.Tools;

namespace Phazor.Components;

public abstract class PhazorComponent : ComponentBase, ICssClassDirector, ICssStyleDirector
{
    private readonly CssStyleFactory _styleFactory;
    private readonly CssClassFactory _classFactory;

    protected PhazorComponent()
    {
        _styleFactory = new CssStyleFactory(Direct);
        _classFactory = new CssClassFactory(Direct);
    }

    protected string ComputedStyle => _styleFactory.Value;
    protected string ComputedClass => _classFactory.Value;

    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string? Width { get; set; }

    [Parameter]
    public string? Height { get; set; }

    public CssClassBuilder Direct(CssClassBuilder builder)
    {
        builder
            .AddWhenNotNull(Class)
            .Add("phazor-component");

        ConfigureClasses(builder);

        return builder;
    }

    public CssStyleBuilder Direct(CssStyleBuilder builder)
    {
        builder
            .AddWhenNotNull(Style)
            .AddWhenValueNotNull("width", Width)
            .AddWhenValueNotNull("height", Height);

        ConfigureStyle(builder);

        return builder;
    }

    public sealed override async Task SetParametersAsync(ParameterView parameters)
    {
        bool styleChanged =
            parameters.TryGetUpdatedValue(nameof(Style), Style, out _)
            || parameters.TryGetUpdatedValue(nameof(Width), Width, out _)
            || parameters.TryGetUpdatedValue(nameof(Height), Height, out _);

        bool classChanged = parameters.TryGetUpdatedValue(nameof(Class), Class, out _);

        PhazorSetParametersResult beforeResult = await BeforeSetParametersAsync(parameters);
        await base.SetParametersAsync(parameters);
        PhazorSetParametersResult afterResult = await AfterSetParametersAsync(parameters);

        if (styleChanged || beforeResult.StyleChanged || afterResult.StyleChanged)
            OnStyleChanged();

        if (classChanged || beforeResult.ClassChanged || afterResult.ClassChanged)
            OnClassChanged();
    }

    protected void OnStyleChanged() => _styleFactory.Invalidate();

    protected void OnClassChanged() => _classFactory.Invalidate();

    protected virtual void ConfigureClasses(CssClassBuilder builder) { }

    protected virtual void ConfigureStyle(CssStyleBuilder builder) { }

    protected virtual ValueTask<PhazorSetParametersResult> BeforeSetParametersAsync(ParameterView parameters)
        => ValueTask.FromResult(new PhazorSetParametersResult(StyleChanged: false, ClassChanged: false));

    protected virtual ValueTask<PhazorSetParametersResult> AfterSetParametersAsync(ParameterView parameters)
        => ValueTask.FromResult(new PhazorSetParametersResult(StyleChanged: false, ClassChanged: false));
}
