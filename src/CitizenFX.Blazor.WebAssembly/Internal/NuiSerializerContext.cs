using System.Text.Json.Serialization;

namespace CitizenFX.Blazor.WebAssembly.Internal;

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = false)]
[JsonSerializable(typeof(object))]
public partial class NuiSerializerContext : JsonSerializerContext
{
    
}
