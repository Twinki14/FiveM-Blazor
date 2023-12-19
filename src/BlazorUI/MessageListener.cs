using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace BlazorUI;

public class MessageListener : ComponentBase
{
    [Inject]
    private ILogger<MessageListener> Logger { get; set; }

    private string? _assemblyName;
    private string? _nuiEventMethod;

    private static MessageListener? _instance;
    private static ILogger<MessageListener>? _logger;

    protected override void OnInitialized()
    {
        if (_instance != null)
        {
            throw new InvalidOperationException($"{nameof(MessageListener)} should only ever be instantiated once.");
        }

        _assemblyName = typeof(MessageListener).Assembly.GetName().Name;
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
            DotNet.invokeMethod('{_assemblyName}', '{_nuiEventMethod}', event);
        }});");
        
        builder.CloseElement();
    }

    [JSInvokable]
    public static void OnNuiEvent(dynamic eventData)
    {
        string json = JsonSerializer.Serialize(eventData);
        
        _logger?.LogInformation("OnNuiEvent received {EventData}", json);
    }
}
