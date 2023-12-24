using System.Text.Json;
using CitizenFX.Blazor.WebAssembly.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CitizenFX.Blazor.WebAssembly;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNuiServices(
        this IServiceCollection services, 
        Action<JsonSerializerOptions>? configureNuiJsonSerializerOptions = null)
    {
        configureNuiJsonSerializerOptions?.Invoke(NuiJsonSerializerOptions.Options);
        
        services.TryAddSingleton<INuiCallbackService, NuiCallbackService>();
        
        return services;
    }
}
