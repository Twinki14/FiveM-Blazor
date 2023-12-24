namespace CitizenFX.Blazor.WebAssembly.Services;

public interface INuiCallbackService
{
    ValueTask<HttpResponseMessage> TriggerNuiCallbackAsync<T>(string callback, T value, CancellationToken cancellationToken = default);
}
