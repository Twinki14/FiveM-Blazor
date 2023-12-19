using System.Text.Json;
using CitizenFX.Core.Native;
using Microsoft.JSInterop;
using RestSharp;

namespace BlazorUI;

public static class Communicator
{
    public static string AppStyle = "display: none";

    private static Dictionary<string, List<Action<string>>> _events = new();
    private static string ParentResourceName { get; set; }

    public static string TriggerNuiCallback(string name, dynamic data)
    {
        API.GetCurrentResourceName();
        
        string json = JsonSerializer.Serialize(data);
        Console.WriteLine($"Triggering nui callback {name}: {json}");

        var client = new RestClient($"https://{ParentResourceName}");

        var request = new RestRequest($"/{name}", Method.Post);
        request.AddParameter("application/json", json, ParameterType.RequestBody);

        var response = client.Execute(request);

        Console.WriteLine(response.Content);
        return response.Content;
    }

    /*[JSInvokable("OnNuiEvent")]
    public static void OnNuiEvent(string name, string data)
    {
        Console.WriteLine("Got event " + name);
        if (!_events.ContainsKey(name)) return;

        foreach (var callback in _events[name])
            callback.Invoke(data);
    }*/

    [JSInvokable("SetResourceName ")]
    public static void SetResourceName(string name)
    {
        ParentResourceName = name;
    }

    public static void AddEventHandler(string name, Action<string> callback)
    {
        if (!_events.ContainsKey(name))
            _events[name] = new List<Action<string>>();

        Console.WriteLine("Adding event handler for " + name);

        _events[name].Add(callback);
    }
}
