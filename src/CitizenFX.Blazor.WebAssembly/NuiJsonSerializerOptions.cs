using System.Text.Json;

namespace CitizenFX.Blazor.WebAssembly;

internal static class NuiJsonSerializerOptions
{
    internal static JsonSerializerOptions Options { get; set; } = new();
}
