using ArkServerManager.Admin.Components;
using ArkServerManager.Admin.Configuration;
using ArkServerManager.Admin.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables(prefix: "ARKMGR_ADMIN_");

builder.Services.Configure<AdminListenOptions>(builder.Configuration.GetSection(AdminListenOptions.SectionName));
builder.Services.Configure<WorkerApiOptions>(builder.Configuration.GetSection(WorkerApiOptions.SectionName));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddTransient<WorkerApiKeyHeaderHandler>();
builder.Services.AddHttpClient<WorkerApiClient>((serviceProvider, client) =>
    {
        var options = serviceProvider.GetRequiredService<IOptions<WorkerApiOptions>>().Value;
        client.BaseAddress = new Uri(options.BaseUrl, UriKind.Absolute);
        client.Timeout = TimeSpan.FromSeconds(Math.Max(options.TimeoutSeconds, 1));
    })
    .AddHttpMessageHandler<WorkerApiKeyHeaderHandler>();

var app = builder.Build();

var listenOptions = app.Services.GetRequiredService<IOptions<AdminListenOptions>>().Value;
if (listenOptions.Public)
{
    app.Logger.LogWarning("Listen:Public is enabled for Admin UI. The UI is exposed beyond loopback.");
    app.Urls.Add($"http://0.0.0.0:{listenOptions.Port}");
}
else
{
    app.Urls.Add($"http://127.0.0.1:{listenOptions.Port}");
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
