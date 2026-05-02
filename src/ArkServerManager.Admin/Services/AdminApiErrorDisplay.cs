using System.Net;

namespace ArkServerManager.Admin.Services;

public static class AdminApiErrorDisplay
{
    public static string Describe(ApiError? error, HttpStatusCode statusCode)
    {
        if (error?.Code == "WORKER_UNREACHABLE")
        {
            return $"{error.Message} Start ArkServerManager.Worker on http://127.0.0.1:5080 before opening Admin UI.";
        }

        if (error?.Code == "WORKER_TIMEOUT")
        {
            return "Worker API did not respond in time. Check Worker health and retry.";
        }

        if ((int)statusCode == 401)
        {
            return "Worker API rejected the request with 401 Unauthorized. Check Worker:ApiKey in admin configuration or user-secrets.";
        }

        if ((int)statusCode == 409)
        {
            return $"Worker API reported a conflict (409): {error?.Message ?? "job cannot run in current server state."}";
        }

        if (error is not null)
        {
            return $"{error.Code}: {error.Message}";
        }

        return $"Worker API returned {(int)statusCode}.";
    }
}
