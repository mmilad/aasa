using ArkServerManager.Worker.Domain;

namespace ArkServerManager.Worker.Contracts;

public sealed record ErrorEnvelope(ApiError Error);
public sealed record ApiError(string Code, string Message, object? Detail = null);

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

public sealed record CreateServerRequest(
    string Name,
    string MapName,
    string SessionName,
    int? GamePort,
    int? QueryPort,
    int? RconPort,
    string? RconPassword);

public sealed record ServerListResponse(IReadOnlyList<ServerSummaryResponse> Items);

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

public sealed record HealthResponse(string Status, string Version);

public sealed record ServerStatusResponse(
    ServerState State,
    int? Pid,
    int GamePort,
    int QueryPort,
    int RconPort);

public sealed record LogsResponse(IReadOnlyList<string> Lines, bool Truncated);
