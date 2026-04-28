using ArkServerManager.Worker.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ArkServerManager.Worker.Api;

public static class ApiErrorResponses
{
    public static JsonHttpResult<ErrorEnvelope> Envelope(
        string code,
        string message,
        object? detail = null,
        int statusCode = StatusCodes.Status400BadRequest)
    {
        return TypedResults.Json(
            new ErrorEnvelope(new ApiError(code, message, detail)),
            statusCode: statusCode);
    }

    public static JsonHttpResult<ErrorEnvelope> Envelope(ApiError apiError, int statusCode = StatusCodes.Status400BadRequest)
    {
        return TypedResults.Json(
            new ErrorEnvelope(apiError),
            statusCode: statusCode);
    }
}
