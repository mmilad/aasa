using ArkServerManager.Worker.Contracts;
using Microsoft.Extensions.Logging;

namespace ArkServerManager.Worker.Services;

public sealed class FakeProcessSupervisor(ILogger<FakeProcessSupervisor> logger) : IProcessSupervisor
{
    public Task<ProcessExecutionResult> StartAsync(
        Guid serverId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        // TODO(WIN-SMOKE): Replace with real ArkAscendedServer.exe process launching on Windows host.
        logger.LogInformation("Executing fake start for server {ServerId}", serverId);
        return Task.FromResult(new ProcessExecutionResult(0, $"Stubbed Start for {serverId:D}"));
    }

    public Task<ProcessExecutionResult> StopAsync(Guid serverId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        // TODO(WIN-SMOKE): Replace with real RCON stop + process kill fallback on Windows host.
        logger.LogInformation("Executing fake stop for server {ServerId}", serverId);
        return Task.FromResult(new ProcessExecutionResult(0, $"Stubbed Stop for {serverId:D}"));
    }

    public async Task<ProcessExecutionResult> RestartAsync(Guid serverId, CancellationToken cancellationToken = default)
    {
        var stop = await StopAsync(serverId, cancellationToken);
        if (stop.ExitCode != 0)
        {
            return stop;
        }

        return await StartAsync(serverId, cancellationToken);
    }
}
