using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Phazor.Components.Tools;

namespace Phazor.Components.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPhazorComponents(
        this IServiceCollection collection,
        Action<PhazorComponentsOptions>? config = null)
    {
        OptionsBuilder<PhazorComponentsOptions> optionsBuilder = collection.AddOptions<PhazorComponentsOptions>();

        if (config is not null)
        {
            optionsBuilder.Configure(config);
        }

        return collection;
    }
}