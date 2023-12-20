using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace CitizenFX.Blazor.WebAssembly;

public class NuiMessageListener : ComponentBase
{
    [Inject]
    private ILogger<NuiMessageListener> Logger { get; set; }

    private string? _assemblyName;
    private string? _nuiEventMethod;

    private static NuiMessageListener? _instance;
    private static ILogger<NuiMessageListener>? _logger;

    protected override void OnInitialized()
    {
        if (_instance != null)
        {
            throw new InvalidOperationException($"{nameof(NuiMessageListener)} should only ever be instantiated once.");
        }

        _assemblyName = typeof(NuiMessageListener).Assembly.GetName().Name;
        _nuiEventMethod = nameof(OnNuiEvent);

        _instance = this;
        _logger = Logger;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "script");
        builder.AddAttribute(1, "type", "text/javascript");
        
        builder.AddMarkupContent(2, $@"
        window.addEventListener('message', (event) => {{
            DotNet.invokeMethod('{_assemblyName}', '{_nuiEventMethod}', event.data);
        }});");
        
        builder.CloseElement();
    }

    [JSInvokable]
    public static void OnNuiEvent(JsonDocument eventData)
    {
        var methods = NuiComponent.FindMethods();

        if (!eventData.RootElement.TryGetProperty(NuiMessageHandler.Identifier, out var identifierValue))
        {
            return;
        }
        
        var identifiedMethods = methods.Where(m => m.Type == identifierValue.GetString()).ToList();
        
        if (identifiedMethods.Count > 1)
        {
            throw new InvalidOperationException("");
        }

        var first = identifiedMethods.First();
        
        // Loop through the method params of first
        // if its not found in the json, throw exception and exit
        // if it is found, deserialize and add to a list of param values to be invoked with
    }
}
