using ArkServerManager.Worker.Contracts;
using ArkServerManager.Worker.Data.Entities;
using ArkServerManager.Worker.Data.Repositories;
using ArkServerManager.Worker.Domain;
using ArkServerManager.Worker.Infrastructure;
using Microsoft.Extensions.Logging;

namespace ArkServerManager.Worker.Services;

public interface IServerApplicationService
{
    Task<IReadOnlyList<ServerEntity>> ListServersAsync(CancellationToken cancellationToken = default);
    Task<ServerEntity?> GetServerAsync(Guid serverId, CancellationToken cancellationToken = default);
    Task<(ServerEntity? Server, ApiError? Error)> CreateServerAsync(
        CreateServerRequest request,
        CancellationToken cancellationToken = default);
    Task<bool> DeleteServerAsync(Guid serverId, CancellationToken cancellationToken = default);
    Task<(JobEntity? Job, ApiError? Error)> EnqueueJobAsync(
        Guid serverId,
        JobType jobType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Runs work for a job that was persisted as <see cref="JobStatus.Running"/> and queued by <see cref="EnqueueJobAsync"/>.
    /// </summary>
    Task ProcessQueuedJobAsync(Guid jobId, CancellationToken cancellationToken = default);

    Task<JobEntity?> GetJobAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task<LogsResponse?> GetLogsAsync(Guid serverId, int tail, CancellationToken cancellationToken = default);
}

public sealed class ServerApplicationService(
    IServerRepository serverRepository,
    IJobRepository jobRepository,
    IDataDirectoryService dataDirectoryService,
    IPortAvailabilityService portAvailabilityService,
    ISteamCmdClient steamCmdClient,
    IProcessSupervisor processSupervisor,
    IJobWorkQueue jobWorkQueue,
    ILogger<ServerApplicationService> logger) : IServerApplicationService
{
    private const int DefaultGamePort = 7777;
    private const int DefaultQueryPort = 27015;
    private const int DefaultRconPort = 27020;

    public Task<IReadOnlyList<ServerEntity>> ListServersAsync(CancellationToken cancellationToken = default)
    {
        return serverRepository.ListAsync(cancellationToken);
    }

    public Task<ServerEntity?> GetServerAsync(Guid serverId, CancellationToken cancellationToken = default)
    {
        return serverRepository.GetAsync(serverId, cancellationToken);
    }

    public async Task<(ServerEntity? Server, ApiError? Error)> CreateServerAsync(
        CreateServerRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return (null, new ApiError("VALIDATION_FAILED", "Name is required."));
        }

        if (string.IsNullOrWhiteSpace(request.MapName))
        {
            return (null, new ApiError("VALIDATION_FAILED", "MapName is required."));
        }

        if (string.IsNullOrWhiteSpace(request.SessionName))
        {
            return (null, new ApiError("VALIDATION_FAILED", "SessionName is required."));
        }

        var gamePort = request.GamePort ?? DefaultGamePort;
        var queryPort = request.QueryPort ?? DefaultQueryPort;
        var rconPort = request.RconPort ?? DefaultRconPort;

        var ports = new[] { gamePort, queryPort, rconPort };
        var unavailable = ports.FirstOrDefault(x => !portAvailabilityService.IsAvailable(x));
        if (unavailable != 0)
        {
            return (
                null,
                new ApiError(
                    "VALIDATION_FAILED",
                    $"Port {unavailable} is unavailable.",
                    new { field = "port", port = unavailable }));
        }

        var now = DateTime.UtcNow;
        var serverId = Guid.NewGuid();
        var server = new ServerEntity
        {
            Id = serverId,
            Name = request.Name.Trim(),
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
            State = ServerState.Stopped,
            InstallRoot = dataDirectoryService.GetServerBinariesRoot(serverId),
            GamePort = gamePort,
            QueryPort = queryPort,
            RconPort = rconPort,
            RconPassword = request.RconPassword ?? string.Empty,
            MapName = request.MapName.Trim(),
            SessionName = request.SessionName.Trim(),
        };

        await serverRepository.AddAsync(server, cancellationToken);
        await serverRepository.SaveChangesAsync(cancellationToken);
        await dataDirectoryService.EnsureServerTreeAsync(serverId, cancellationToken);
        return (server, null);
    }

    public async Task<bool> DeleteServerAsync(Guid serverId, CancellationToken cancellationToken = default)
    {
        var deleted = await serverRepository.DeleteAsync(serverId, cancellationToken);
        if (!deleted)
        {
            return false;
        }

        await serverRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<(JobEntity? Job, ApiError? Error)> EnqueueJobAsync(
        Guid serverId,
        JobType jobType,
        CancellationToken cancellationToken = default)
    {
        var server = await serverRepository.GetAsync(serverId, cancellationToken);
        if (server is null)
        {
            return (null, new ApiError("NOT_FOUND", "Server was not found."));
        }

        if (jobType == JobType.InstallOrUpdate && server.State != ServerState.Stopped)
        {
            return (null, new ApiError("SERVER_NOT_STOPPED", "Install requires server to be stopped."));
        }

        if (jobType == JobType.Backup && server.State != ServerState.Stopped)
        {
            return (null, new ApiError("SERVER_NOT_STOPPED", "Backup requires server to be stopped."));
        }

        if (await jobRepository.HasRunningJobForServerAsync(serverId, cancellationToken))
        {
            return (null, new ApiError("CONFLICT", "Another running job exists for this server."));
        }

        if (jobType == JobType.InstallOrUpdate && await jobRepository.HasRunningInstallAsync(cancellationToken))
        {
            return (null, new ApiError("CONFLICT", "Another install job is already running globally."));
        }

        var now = DateTime.UtcNow;
        var job = new JobEntity
        {
            Id = Guid.NewGuid(),
            ServerId = serverId,
            Type = jobType,
            Status = JobStatus.Running,
            CreatedAtUtc = now,
            StartedAtUtc = now,
        };

        await jobRepository.AddAsync(job, cancellationToken);
        server.LastJobId = job.Id;
        server.UpdatedAtUtc = now;
        await jobRepository.SaveChangesAsync(cancellationToken);
        await serverRepository.SaveChangesAsync(cancellationToken);

        // Do not pass the HTTP request token: the client disconnects after 202 while work continues.
        await jobWorkQueue.EnqueueAsync(job.Id, CancellationToken.None);

        return (job, null);
    }

    public async Task ProcessQueuedJobAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        var job = await jobRepository.GetTrackedAsync(jobId, cancellationToken);
        if (job is null)
        {
            logger.LogWarning("Queued job {JobId} was not found; skipping.", jobId);
            return;
        }

        if (job.Status != JobStatus.Running)
        {
            return;
        }

        try
        {
            var runResult = await ExecuteJobAsync(job.Type, job.ServerId, cancellationToken);
            job.Status = runResult.ExitCode is 0 ? JobStatus.Done : JobStatus.Failed;
            job.ExitCode = runResult.ExitCode;
            job.LogBlob = runResult.Log;
            job.FinishedAtUtc = DateTime.UtcNow;
        }
        catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogError(exception, "Job {JobId} failed with an exception.", jobId);
            job.Status = JobStatus.Failed;
            job.ExitCode = 1;
            job.LogBlob = exception.ToString();
            job.FinishedAtUtc = DateTime.UtcNow;
        }

        await jobRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task<(int ExitCode, string? Log)> ExecuteJobAsync(
        JobType jobType,
        Guid serverId,
        CancellationToken cancellationToken)
    {
        switch (jobType)
        {
            case JobType.InstallOrUpdate:
            {
                var steamResult = await steamCmdClient.InstallOrUpdateAsync(serverId, cancellationToken);
                return (steamResult.ExitCode, steamResult.Output);
            }
            case JobType.Start:
            {
                var processResult = await processSupervisor.StartAsync(serverId, cancellationToken);
                return (processResult.ExitCode, processResult.Output);
            }
            case JobType.Stop:
            {
                var processResult = await processSupervisor.StopAsync(serverId, cancellationToken);
                return (processResult.ExitCode, processResult.Output);
            }
            case JobType.Restart:
            {
                var processResult = await processSupervisor.RestartAsync(serverId, cancellationToken);
                return (processResult.ExitCode, processResult.Output);
            }
            case JobType.Backup:
                // TODO(WIN-SMOKE): Replace with real backup zip job over SavedArks + WindowsServer INI files.
                return (0, "Stubbed backup job output.");
            default:
                return (1, $"Unknown job type {jobType}.");
        }
    }

    public Task<JobEntity?> GetJobAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        return jobRepository.GetAsync(jobId, cancellationToken);
    }

    public async Task<LogsResponse?> GetLogsAsync(Guid serverId, int tail, CancellationToken cancellationToken = default)
    {
        var server = await serverRepository.GetAsync(serverId, cancellationToken);
        if (server is null)
        {
            return null;
        }

        var logPath = Path.Combine(dataDirectoryService.GetServerRoot(serverId), "logs", "server.log");
        if (!File.Exists(logPath))
        {
            return new LogsResponse([], false);
        }

        var lines = await File.ReadAllLinesAsync(logPath, cancellationToken);
        var normalizedTail = Math.Clamp(tail, 1, 500);
        var truncated = lines.Length > normalizedTail;
        var selected = lines.Skip(Math.Max(0, lines.Length - normalizedTail)).ToArray();
        return new LogsResponse(selected, truncated);
    }
}
