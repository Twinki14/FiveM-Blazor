using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace CitizenFX.Blazor.WebAssembly;

public partial class NuiMessageListener : ComponentBase
{
    [Inject]
    private ILogger<NuiMessageListener> Logger { get; set; }

    private string? _assemblyName;
    private string? _nuiMessageMethod;

    private static NuiMessageListener? _instance;
    private static ILogger<NuiMessageListener>? _logger;

    public static JsonSerializerOptions JsonSerializerOptions = new(); 

    protected override void OnInitialized()
    {
        if (_instance != null)
        {
            throw new InvalidOperationException($"{nameof(NuiMessageListener)} should only ever be instantiated once.");
        }

        _assemblyName = typeof(NuiMessageListener).Assembly.GetName().Name;
        _nuiMessageMethod = nameof(OnNuiMessage);

        _instance = this;
        _logger = Logger;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "script");
        builder.AddAttribute(1, "type", "text/javascript");
        
        builder.AddMarkupContent(2, $@"
        window.addEventListener('message', (event) => {{
            DotNet.invokeMethod('{_assemblyName}', '{_nuiMessageMethod}', event.data);
        }});");
        
        builder.CloseElement();
    }

    [JSInvokable]
    public static void OnNuiMessage(JsonDocument eventData)
    {
        var methods = NuiComponent.FindMethods();

        if (!eventData.RootElement.TryGetProperty(NuiMessageHandler.Identifier, out var identifierValue))
        {
            return;
        }
        
        var identifiedMethods = methods.Where(m => m.Type == identifierValue.GetString()).ToList();
        
        // Optionally, if it's > 1, then check and make sure all identified methods have the same number of params and all params are have the same types and names
        if (identifiedMethods.Count > 1)
        {
            throw new InvalidOperationException($"More than one method is attached to the {nameof(NuiMessageHandler)} attribute with the type identifier {identifierValue}");
        }

        var identifiedMethod = identifiedMethods.First();
        var methodParams = identifiedMethod.MethodInfo.GetParameters();
        var methodValues = new List<object>();

        foreach (var param in methodParams)
        {
            try
            {
                if (param.Name is null)
                {
                    throw new Exception();
                }
                
                if (eventData.RootElement.TryGetProperty(param.Name, out var element))
                {
                    var deserialized = element.Deserialize(param.ParameterType, JsonSerializerOptions);
                    if (deserialized is not null)
                    {
                        methodValues.Add(deserialized);
                    }
                    else
                    {
                        throw new InvalidOperationException("Deserialized object is null, this isn't expected");
                    }
                }
                else
                {
                    LogErrorPropertyNotFound(_logger!, param.Name, param.ParameterType);
                    return;
                }
            }
            catch (Exception e)
            {
                LogCriticalJsonBinding(_logger!, e, param.Name, param.ParameterType);
                return;
            }
        }
        
        identifiedMethod.MethodInfo.Invoke(identifiedMethod.Instance, methodValues.ToArray());
    }
    
    [LoggerMessage(1, LogLevel.Critical, "Critical exception in {caller} when attempting to bind to a handler method with a parameter name {parameterName} and parameter type {parameterType}", EventName = "Handler method parameter JSON binding")]
    static partial void LogCriticalJsonBinding(ILogger logger, Exception ex, string? parameterName, Type parameterType, [CallerMemberName] string caller = nameof(NuiMessageListener));
    
    [LoggerMessage(2, LogLevel.Critical, "Parameter not found in the handler when attempting to bind with a parameter name {parameterName} and parameter type {parameterType}", EventName = "Handler method parameter discovery")]
    static partial void LogErrorPropertyNotFound(ILogger logger, string? parameterName, Type parameterType, [CallerMemberName] string caller = nameof(NuiMessageListener));
}
