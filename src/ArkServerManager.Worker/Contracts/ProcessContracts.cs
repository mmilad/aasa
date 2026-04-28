namespace ArkServerManager.Worker.Contracts;

public sealed record ProcessExecutionResult(int ExitCode, string? Output = null);

public interface IProcessSupervisor
{
    Task<ProcessExecutionResult> StartAsync(Guid serverId, CancellationToken cancellationToken = default);
    Task<ProcessExecutionResult> StopAsync(Guid serverId, CancellationToken cancellationToken = default);
    Task<ProcessExecutionResult> RestartAsync(Guid serverId, CancellationToken cancellationToken = default);
}
