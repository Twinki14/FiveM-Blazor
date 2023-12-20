using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace CitizenFX.Blazor.WebAssembly;

public class NuiComponent : ComponentBase
{
    private static readonly List<NuiComponent> Instances = new();
    
    public NuiComponent()
    {
        // Add the current instance to the list when instantiated
        Instances.Add(this);
        
        // Add an lock pattern here for caching methods
        // so that we re-generate the known-methods cache whenever a new instance is added,
        // but it needs to be thread-safe
        // this needs to work with async as well?
    }
    
    internal readonly struct MethodInfoWithInstance(MethodInfo methodInfo, object? instance, string type)
    {
        public MethodInfo MethodInfo { get; } = methodInfo;
        public object? Instance { get; } = instance;
        public string Type { get; } = type;
    }

    internal static List<MethodInfoWithInstance> FindMethods()
    {
        var methods = new List<MethodInfoWithInstance>();

        foreach (var instance in Instances)
        {
            var methodsWithAttribute = instance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => Attribute.IsDefined(m, typeof(NuiMessageHandler)))
                .Select(m =>
                {
                    var attribute = (NuiMessageHandler) Attribute.GetCustomAttribute(m, typeof(NuiMessageHandler))!;
                    return new MethodInfoWithInstance(m, instance, attribute.Type);
                })
                .ToList();

            methods.AddRange(methodsWithAttribute);
        }

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            var staticMethodsInAssembly = assembly.GetTypes()
                .Where(type => typeof(NuiComponent).IsAssignableFrom(type) && type != typeof(NuiComponent))
                .SelectMany(type => type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(method => Attribute.IsDefined(method, typeof(NuiMessageHandler)))
                    .Select(method =>
                    {
                        var attribute = (NuiMessageHandler) Attribute.GetCustomAttribute(method, typeof(NuiMessageHandler))!;
                        return new MethodInfoWithInstance(method, null, attribute.Type);
                    }))
                .ToList();

            methods.AddRange(staticMethodsInAssembly);
        }

        return methods;
    }
}
