global using FarmerbotWebUI.Server.Services.Docker;
global using FarmerbotWebUI.Server.Services.Filesystem;
global using FarmerbotWebUI.Shared;
using FarmerbotWebUI.Server.Services.TfApiClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddHttpClient<TfGraphQLApiClient>(client =>
{
    client.BaseAddress = new Uri("https://graphql.qa.grid.tf/graphql");
});
//builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IDockerService, DockerService>();
builder.Services.AddScoped<ITfGraphQLApiClient, TfGraphQLApiClient>();

//builder.Services.AddScoped<IDockerService>(provider =>
//{
//    return new DockerService(builder.Configuration);
//});

var app = builder.Build();

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
