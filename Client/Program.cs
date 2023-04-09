global using FarmerbotWebUI.Shared;
using FarmerbotWebUI.Client;
using FarmerbotWebUI.Client.Services.Docker;
using FarmerbotWebUI.Client.Services.EventConsole;
using FarmerbotWebUI.Client.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<IDockerService, DockerService>();
builder.Services.AddScoped<IEventConsoleService, EventConsoleService>();
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();
builder.Services.AddScoped<EventConsole>();
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<MainLayout>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });


await builder.Build().RunAsync();
