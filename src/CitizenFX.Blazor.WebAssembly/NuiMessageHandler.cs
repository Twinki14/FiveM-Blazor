namespace CitizenFX.Blazor.WebAssembly;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class NuiMessageHandler(string type) : Attribute
{
    public readonly string Type = type;
    public const string Identifier = "type";
}
