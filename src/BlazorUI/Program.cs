using BlazorUI;
using CitizenFX.Blazor.WebAssembly;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.RootComponents.Add<NuiMessageListener>("#nui-message-listener");

builder.Services.AddNuiServices();
builder.Services.AddRadzenComponents();

await builder.Build().RunAsync();
