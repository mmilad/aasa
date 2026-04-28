using ArkServerManager.Worker.Contracts;
using ArkServerManager.Worker.Data.Entities;
using ArkServerManager.Worker.Domain;
using ArkServerManager.Worker.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ArkServerManager.Worker.Api;

public static class V1Endpoints
{
    public static void MapV1Api(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1").WithTags("v1");

        group.MapGet("/servers", ListServersAsync);
        group.MapPost("/servers", CreateServerAsync);
        group.MapGet("/servers/{serverId:guid}", GetServerAsync);
        group.MapDelete("/servers/{serverId:guid}", DeleteServerAsync);

        group.MapPost("/servers/{serverId:guid}/jobs/install", EnqueueInstallAsync);
        group.MapPost("/servers/{serverId:guid}/jobs/backup", EnqueueBackupAsync);
        group.MapPost("/servers/{serverId:guid}/actions/start", EnqueueStartAsync);
        group.MapPost("/servers/{serverId:guid}/actions/stop", EnqueueStopAsync);
        group.MapPost("/servers/{serverId:guid}/actions/restart", EnqueueRestartAsync);

        group.MapGet("/servers/{serverId:guid}/status", GetServerStatusAsync);
        group.MapGet("/servers/{serverId:guid}/logs", GetLogsAsync);
        group.MapGet("/jobs/{jobId:guid}", GetJobAsync);

        group.MapGet("/servers/{serverId:guid}/ini/GameUserSettings", GetGameUserSettingsAsync);
        group.MapGet("/servers/{serverId:guid}/ini/Game.ini", GetGameIniAsync);
        group.MapPatch("/servers/{serverId:guid}/ini/GameUserSettings", PatchGameUserSettingsAsync);
    }

    private static async Task<Ok<ServerListResponse>> ListServersAsync(
        IServerApplicationService service,
        CancellationToken cancellationToken)
    {
        var items = await service.ListServersAsync(cancellationToken);
        return TypedResults.Ok(new ServerListResponse(items.Select(MapSummary).ToArray()));
    }

    private static async Task<Results<Created<ServerResponse>, JsonHttpResult<ErrorEnvelope>>> CreateServerAsync(
        CreateServerRequest request,
        IServerApplicationService service,
        CancellationToken cancellationToken)
    {
        var result = await service.CreateServerAsync(request, cancellationToken);
        if (result.Error is not null)
        {
            return ApiErrorResponses.Envelope(result.Error, StatusCodes.Status400BadRequest);
        }

        return TypedResults.Created($"/api/v1/servers/{result.Server!.Id:D}", MapServer(result.Server));
    }

    private static async Task<Results<Ok<ServerResponse>, JsonHttpResult<ErrorEnvelope>>> GetServerAsync(
        Guid serverId,
        IServerApplicationService service,
        CancellationToken cancellationToken)
    {
        var server = await service.GetServerAsync(serverId, cancellationToken);
        if (server is null)
        {
            return ApiErrorResponses.Envelope("NOT_FOUND", "Server was not found.", statusCode: StatusCodes.Status404NotFound);
        }

        return TypedResults.Ok(MapServer(server));
    }

    private static async Task<Results<NoContent, JsonHttpResult<ErrorEnvelope>>> DeleteServerAsync(
        Guid serverId,
        IServerApplicationService service,
        CancellationToken cancellationToken)
    {
        var deleted = await service.DeleteServerAsync(serverId, cancellationToken);
        if (!deleted)
        {
            return ApiErrorResponses.Envelope("NOT_FOUND", "Server was not found.", statusCode: StatusCodes.Status404NotFound);
        }

        return TypedResults.NoContent();
    }

    private static Task<Results<Accepted<JobAcceptedResponse>, JsonHttpResult<ErrorEnvelope>>> EnqueueInstallAsync(
        Guid serverId,
        IServerApplicationService service,
        CancellationToken cancellationToken)
    {
        return EnqueueJobAsync(serverId, JobType.InstallOrUpdate, service, cancellationToken);
    }

    private static Task<Results<Accepted<JobAcceptedResponse>, JsonHttpResult<ErrorEnvelope>>> EnqueueBackupAsync(
        Guid serverId,
        IServerApplicationService service,
        CancellationToken cancellationToken)
    {
        return EnqueueJobAsync(serverId, JobType.Backup, service, cancellationToken);
    }

    private static Task<Results<Accepted<JobAcceptedResponse>, JsonHttpResult<ErrorEnvelope>>> EnqueueStartAsync(
        Guid serverId,
        IServerApplicationService service,
        CancellationToken cancellationToken)
    {
        return EnqueueJobAsync(serverId, JobType.Start, service, cancellationToken);
    }

    private static Task<Results<Accepted<JobAcceptedResponse>, JsonHttpResult<ErrorEnvelope>>> EnqueueStopAsync(
        Guid serverId,
        IServerApplicationService service,
        CancellationToken cancellationToken)
    {
        return EnqueueJobAsync(serverId, JobType.Stop, service, cancellationToken);
    }

    private static Task<Results<Accepted<JobAcceptedResponse>, JsonHttpResult<ErrorEnvelope>>> EnqueueRestartAsync(
        Guid serverId,
        IServerApplicationService service,
        CancellationToken cancellationToken)
    {
        return EnqueueJobAsync(serverId, JobType.Restart, service, cancellationToken);
    }

    private static async Task<Results<Accepted<JobAcceptedResponse>, JsonHttpResult<ErrorEnvelope>>> EnqueueJobAsync(
        Guid serverId,
        JobType jobType,
        IServerApplicationService service,
        CancellationToken cancellationToken)
    {
        var result = await service.EnqueueJobAsync(serverId, jobType, cancellationToken);
        if (result.Error is not null)
        {
            var statusCode = result.Error.Code switch
            {
                "NOT_FOUND" => StatusCodes.Status404NotFound,
                "VALIDATION_FAILED" => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status409Conflict,
            };

            return ApiErrorResponses.Envelope(result.Error, statusCode);
        }

        return TypedResults.Accepted($"/api/v1/jobs/{result.Job!.Id:D}", new JobAcceptedResponse(result.Job.Id));
    }

    private static async Task<Results<Ok<ServerStatusResponse>, JsonHttpResult<ErrorEnvelope>>> GetServerStatusAsync(
        Guid serverId,
        IServerApplicationService service,
        CancellationToken cancellationToken)
    {
        var server = await service.GetServerAsync(serverId, cancellationToken);
        if (server is null)
        {
            return ApiErrorResponses.Envelope("NOT_FOUND", "Server was not found.", statusCode: StatusCodes.Status404NotFound);
        }

        return TypedResults.Ok(
            new ServerStatusResponse(
                server.State,
                server.Pid,
                server.GamePort,
                server.QueryPort,
                server.RconPort));
    }

    private static async Task<Results<Ok<LogsResponse>, JsonHttpResult<ErrorEnvelope>>> GetLogsAsync(
        Guid serverId,
        int? tail,
        IServerApplicationService service,
        CancellationToken cancellationToken)
    {
        var response = await service.GetLogsAsync(serverId, tail ?? 200, cancellationToken);
        if (response is null)
        {
            return ApiErrorResponses.Envelope("NOT_FOUND", "Server was not found.", statusCode: StatusCodes.Status404NotFound);
        }

        return TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<JobResponse>, JsonHttpResult<ErrorEnvelope>>> GetJobAsync(
        Guid jobId,
        IServerApplicationService service,
        CancellationToken cancellationToken)
    {
        var job = await service.GetJobAsync(jobId, cancellationToken);
        if (job is null)
        {
            return ApiErrorResponses.Envelope("NOT_FOUND", "Job was not found.", statusCode: StatusCodes.Status404NotFound);
        }

        return TypedResults.Ok(MapJob(job));
    }

    private static async Task<Results<Ok<IniFileResponse>, JsonHttpResult<ErrorEnvelope>>> GetGameUserSettingsAsync(
        Guid serverId,
        IIniConfigurationService iniService,
        CancellationToken cancellationToken)
    {
        var response = await iniService.ReadGameUserSettingsAsync(serverId, cancellationToken);
        if (response is null)
        {
            return ApiErrorResponses.Envelope("NOT_FOUND", "GameUserSettings.ini not found.", statusCode: StatusCodes.Status404NotFound);
        }

        return TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<IniFileResponse>, JsonHttpResult<ErrorEnvelope>>> GetGameIniAsync(
        Guid serverId,
        IIniConfigurationService iniService,
        CancellationToken cancellationToken)
    {
        var response = await iniService.ReadGameIniAsync(serverId, cancellationToken);
        if (response is null)
        {
            return ApiErrorResponses.Envelope("NOT_FOUND", "Game.ini not found.", statusCode: StatusCodes.Status404NotFound);
        }

        return TypedResults.Ok(response);
    }

    private static async Task<Results<Ok<IniFileResponse>, JsonHttpResult<ErrorEnvelope>>> PatchGameUserSettingsAsync(
        Guid serverId,
        PatchIniRequest request,
        IIniConfigurationService iniService,
        CancellationToken cancellationToken)
    {
        var result = await iniService.PatchGameUserSettingsAsync(serverId, request, cancellationToken);
        if (result.Error is not null)
        {
            var statusCode = result.Error.Code switch
            {
                "NOT_FOUND" => StatusCodes.Status404NotFound,
                "KEY_NOT_ALLOWLISTED" => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status400BadRequest,
            };

            return ApiErrorResponses.Envelope(result.Error, statusCode);
        }

        return TypedResults.Ok(result.Response!);
    }

    private static ServerSummaryResponse MapSummary(ServerEntity entity)
    {
        return new ServerSummaryResponse(
            entity.Id,
            entity.Name,
            entity.State,
            entity.GamePort,
            entity.QueryPort,
            entity.RconPort);
    }

    private static ServerResponse MapServer(ServerEntity entity)
    {
        return new ServerResponse(
            entity.Id,
            entity.Name,
            entity.CreatedAtUtc,
            entity.UpdatedAtUtc,
            entity.State,
            entity.InstallRoot,
            entity.GamePort,
            entity.QueryPort,
            entity.RconPort,
            entity.MapName,
            entity.SessionName,
            entity.Pid,
            entity.LastJobId);
    }

    private static JobResponse MapJob(JobEntity entity)
    {
        return new JobResponse(
            entity.Id,
            entity.ServerId,
            entity.Type,
            entity.Status,
            entity.CreatedAtUtc,
            entity.StartedAtUtc,
            entity.FinishedAtUtc,
            entity.ExitCode,
            entity.LogBlob);
    }
}
