using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ArkServerManager.Worker.Configuration;
using ArkServerManager.Worker.Contracts;
using Microsoft.Extensions.Options;

namespace ArkServerManager.Worker.Api;

public sealed class ApiKeyAuthenticationMiddleware(
    RequestDelegate next,
    IOptionsMonitor<ApiAuthOptions> authOptions,
    ILogger<ApiKeyAuthenticationMiddleware> logger)
{
    private const string HeaderName = "X-Api-Key";

    public async Task Invoke(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/api/v1", StringComparison.OrdinalIgnoreCase))
        {
            await next(context);
            return;
        }

        var expected = authOptions.CurrentValue.ApiKey;
        if (string.IsNullOrWhiteSpace(expected))
        {
            logger.LogWarning("API key auth is enabled but ApiAuth:ApiKey is empty.");
            await WriteUnauthorizedAsync(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(HeaderName, out var provided) ||
            !FixedTimeEquals(provided.ToString(), expected))
        {
            await WriteUnauthorizedAsync(context);
            return;
        }

        await next(context);
    }

    private static Task WriteUnauthorizedAsync(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";
        var payload = new ErrorEnvelope(new ApiError("UNAUTHORIZED", "Missing or invalid API key."));
        return context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }

    private static bool FixedTimeEquals(string provided, string expected)
    {
        var providedBytes = Encoding.UTF8.GetBytes(provided);
        var expectedBytes = Encoding.UTF8.GetBytes(expected);
        return CryptographicOperations.FixedTimeEquals(providedBytes, expectedBytes);
    }
}
