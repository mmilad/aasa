using ArkServerManager.Worker.Contracts;

namespace ArkServerManager.Worker.Api;

public static class HealthEndpoints
{
    public static void MapHealthApi(this IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/health",
            (IHostEnvironment environment) =>
            {
                var version = typeof(HealthEndpoints).Assembly.GetName().Version?.ToString() ?? "0.0.0";
                return TypedResults.Ok(new HealthResponse("ok", $"{version}+{environment.EnvironmentName}"));
            })
            .WithName("Health")
            .WithTags("Health");
    }
}
