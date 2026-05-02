using System.Net;

namespace ArkServerManager.Admin.Services;

public enum ServerState
{
    Stopped = 0,
    Starting = 1,
    Running = 2,
    Stopping = 3,
    Updating = 4,
    Crashed = 5,
    Error = 6,
}

public enum JobType
{
    InstallOrUpdate = 0,
    Start = 1,
    Stop = 2,
    Restart = 3,
    Backup = 4,
}

public enum JobStatus
{
    Pending = 0,
    Running = 1,
    Done = 2,
    Failed = 3,
}

public sealed record ErrorEnvelope(ApiError Error);

public sealed record ApiError(string Code, string Message, object? Detail = null);

public sealed record ServerListResponse(IReadOnlyList<ServerSummaryResponse> Items);

public sealed record ServerSummaryResponse(
    Guid Id,
    string Name,
    ServerState State,
    int GamePort,
    int QueryPort,
    int RconPort);

public sealed record ServerResponse(
    Guid Id,
    string Name,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    ServerState State,
    string InstallRoot,
    int GamePort,
    int QueryPort,
    int RconPort,
    string MapName,
    string SessionName,
    int? Pid,
    Guid? LastJobId);

public sealed record JobAcceptedResponse(Guid JobId);

public sealed record JobResponse(
    Guid Id,
    Guid ServerId,
    JobType Type,
    JobStatus Status,
    DateTime CreatedAtUtc,
    DateTime? StartedAtUtc,
    DateTime? FinishedAtUtc,
    int? ExitCode,
    string? LogBlob);

public sealed record ApiClientResponse<T>(
    HttpStatusCode StatusCode,
    T? Value,
    ApiError? Error)
{
    public bool IsSuccess => (int)StatusCode is >= 200 and <= 299;
}

public static class JobStatusNames
{
    public static bool IsTerminal(JobStatus status)
    {
        return status is JobStatus.Done or JobStatus.Failed;
    }
}
