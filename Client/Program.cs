global using FarmerbotWebUI.Client;
global using FarmerbotWebUI.Client.Services.Docker;
global using FarmerbotWebUI.Client.Services.Filesystem;
global using FarmerbotWebUI.Client.Services.EventConsole;
global using FarmerbotWebUI.Client.Services.Settings;
global using FarmerbotWebUI.Client.Services.NodeStatus;
global using FarmerbotWebUI.Client.Shared;
global using FarmerbotWebUI.Shared;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using FarmerBotWebUI.Shared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<IEventConsoleService, EventConsoleService>();
builder.Services.AddScoped<INodeStatusService, NodeStatusService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<IFileService, FilesService>();
builder.Services.AddScoped<IDockerService, DockerService>();
builder.Services.AddSingleton<IAppSettings, AppSettings>();
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();
builder.Services.AddScoped<EventConsole>();
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<MainLayout>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();