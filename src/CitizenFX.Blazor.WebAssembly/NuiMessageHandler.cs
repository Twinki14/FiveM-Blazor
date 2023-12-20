namespace CitizenFX.Blazor.WebAssembly;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class NuiMessageHandler : Attribute
{
    public readonly string Type;
    public const string Identifier = "type";

    public NuiMessageHandler(string type)
    {
        Type = type;
    }
}
