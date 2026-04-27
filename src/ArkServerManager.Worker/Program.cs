using ArkServerManager.Worker.Api;
using ArkServerManager.Worker.Configuration;
using ArkServerManager.Worker.Contracts;
using ArkServerManager.Worker.Data;
using ArkServerManager.Worker.Data.Repositories;
using ArkServerManager.Worker.Infrastructure;
using ArkServerManager.Worker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

if (OperatingSystem.IsWindows())
{
    builder.Host.UseWindowsService();
}
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables(prefix: "ARKMGR_");

builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection(DatabaseOptions.SectionName));
builder.Services.Configure<ApiAuthOptions>(builder.Configuration.GetSection(ApiAuthOptions.SectionName));
builder.Services.Configure<ListenOptions>(builder.Configuration.GetSection(ListenOptions.SectionName));
builder.Services.Configure<ManagerPathsOptions>(options =>
{
    var configured = builder.Configuration.GetValue<string>("ManagerPaths:ManagerDataRoot");
    options.ManagerDataRoot = string.IsNullOrWhiteSpace(configured)
        ? ManagerPathsDefaults.GetDefaultRoot()
        : configured;
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddSingleton<ILoggerProvider, FileLoggerProvider>();

builder.Services.AddDbContext<AppDbContext>((serviceProvider, dbOptions) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
    var managerPaths = serviceProvider.GetRequiredService<IOptions<ManagerPathsOptions>>().Value;
    var provider = options.Provider.Trim();

    if (provider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
    {
        var dataSource = options.Sqlite.DataSource;
        if (!Path.IsPathRooted(dataSource))
        {
            dataSource = Path.Combine(managerPaths.ManagerDataRoot, dataSource);
        }

        var directory = Path.GetDirectoryName(dataSource);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        dbOptions.UseSqlite($"Data Source={dataSource}");
        return;
    }

    if (provider.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
    {
        if (string.IsNullOrWhiteSpace(options.Postgres.ConnectionString))
        {
            throw new InvalidOperationException("Database:Postgres:ConnectionString must be configured for Postgres provider.");
        }

        dbOptions.UseNpgsql(options.Postgres.ConnectionString);
        return;
    }

    throw new InvalidOperationException($"Unsupported database provider '{options.Provider}'. Expected Sqlite or Postgres.");
});

builder.Services.AddScoped<IServerRepository, ServerRepository>();
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IServerApplicationService, ServerApplicationService>();
builder.Services.AddScoped<IIniConfigurationService, IniConfigurationService>();
builder.Services.AddSingleton<IDataDirectoryService, DataDirectoryService>();
builder.Services.AddSingleton<IPortAvailabilityService, PortAvailabilityService>();
builder.Services.AddScoped<ISteamCmdClient, FakeSteamCmdClient>();
builder.Services.AddScoped<IProcessSupervisor, FakeProcessSupervisor>();
builder.Services.AddHostedService<WorkerHealthBeaconService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ArkServerManager.Worker",
        Version = "v1",
    });
    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "X-Api-Key",
        Type = SecuritySchemeType.ApiKey,
        Description = "Shared API key header required for /api/v1 routes.",
    });
    options.OperationFilter<ApiKeySecurityOperationFilter>();
});
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
}

var listenOptions = app.Services.GetRequiredService<IOptions<ListenOptions>>().Value;
if (listenOptions.Public)
{
    app.Logger.LogWarning("Listen:Public is enabled. API is exposed beyond loopback.");
    app.Urls.Add($"http://0.0.0.0:{listenOptions.Port}");
}
else
{
    app.Urls.Add($"http://127.0.0.1:{listenOptions.Port}");
}

app.UseMiddleware<ApiKeyAuthenticationMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();

app.MapHealthApi();
app.MapV1Api();

app.Run();
