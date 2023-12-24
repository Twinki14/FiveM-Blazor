using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace CitizenFX.Blazor.WebAssembly.Services;

public class NuiCallbackService(
    IJSRuntime jsRuntime,
    ILogger<NuiCallbackService> logger)
    : INuiCallbackService
{
    private readonly ILogger<NuiCallbackService> _logger = logger;
    
    private string? ResourceName { get; set; }
    private static readonly SemaphoreSlim ResourceNameSemaphore = new(1, 1);

    public async ValueTask<HttpResponseMessage> TriggerNuiCallbackAsync<T>(string callback, T value, CancellationToken cancellationToken = default)
    {
        var resourceName = await GetResourceNameAsync(cancellationToken);

        using var httpClient = new HttpClient();
        {
            httpClient.BaseAddress = new Uri($"https://{resourceName}/");

            return await httpClient.PostAsJsonAsync($"{callback}", value, NuiJsonSerializerOptions.Options, cancellationToken);   
        }
    }

    private async ValueTask<string> GetResourceNameAsync(CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(ResourceName))
        {
            return ResourceName;
        }

        try
        {
            await ResourceNameSemaphore.WaitAsync(cancellationToken);

            ResourceName = await jsRuntime.InvokeAsync<string>("eval", cancellationToken, "window.location.host");
        }
        finally
        {
            ResourceNameSemaphore.Release();
        }
        
        return ResourceName;
    }
}
