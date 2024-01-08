using Microsoft.AspNetCore.Components;
using Phazor.Components.Tools;

namespace Phazor.Components;

public abstract class PhazorComponent : ComponentBase, ICssClassDirector, ICssStyleDirector
{
    protected string ComputedStyle => new CssStyleBuilder().Use(this).Build();
    protected string ComputedClass => new CssClassBuilder().Use(this).Build();
    
    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string? Width { get; set; } = "100%";

    public string? Height { get; set; } = "100%";

    public CssClassBuilder Direct(CssClassBuilder builder)
    {
        builder.AddWhenNotNull(Class);
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

    protected virtual void ConfigureClasses(CssClassBuilder builder) { }

    protected virtual void ConfigureStyle(CssStyleBuilder builder) { }
}