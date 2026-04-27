using ArkServerManager.Worker.Contracts;
using Microsoft.Extensions.Logging;

namespace ArkServerManager.Worker.Services;

public sealed class FakeSteamCmdClient(ILogger<FakeSteamCmdClient> logger) : ISteamCmdClient
{
    public Task<SteamCmdResult> InstallOrUpdateAsync(Guid serverId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        // TODO(WIN-SMOKE): Replace with real SteamCMD execution (app_update 2430930 validate) on Windows host.
        logger.LogInformation("Executing fake SteamCMD install/update for server {ServerId}", serverId);
        return Task.FromResult(new SteamCmdResult(0, $"Stubbed InstallOrUpdate for {serverId:D}"));
    }
}
