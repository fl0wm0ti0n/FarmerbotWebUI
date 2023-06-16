global using FarmerbotWebUI.Server.Services.Docker;
global using FarmerbotWebUI.Server.Services.Filesystem;
global using FarmerbotWebUI.Server.Services.Settings;
global using FarmerbotWebUI.Server.Services.TfApiClient;
global using FarmerbotWebUI.Server.Services.NodeStatus;
global using FarmerBotWebUI.Shared.NodeStatus;
global using FarmerbotWebUI.Shared.BotConfig;
global using FarmerbotWebUI.Shared;
global using FarmerbotWebUI.Client.Views;
using FarmerbotWebUI.Server;
using FarmerBotWebUI.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IAppSettings, AppSettings>();
builder.Services.AddScoped<ISettingsService, SettingsService>();

builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IDockerService, DockerService>();
builder.Services.AddScoped<ITfGraphQLApiClient, TfGraphQLApiClient>();
builder.Services.AddScoped<INodeStatusService, NodeStatusService>();

var app = builder.Build();

// Startup 

using (var scope1 = app.Services.CreateScope())
{
    var settingsService = scope1.ServiceProvider.GetRequiredService<ISettingsService>();
    _ = await settingsService.ReloadConfiguration();

}
using (var scope2 = app.Services.CreateScope())
{
    var tfGraphQLApiClient = scope2.ServiceProvider.GetRequiredService<ITfGraphQLApiClient>();
    //_ = await tfGraphQLApiClient.GetNodesListAsync(CancellationToken.None);
    _ = await tfGraphQLApiClient.StartStatusInterval();
}

app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
