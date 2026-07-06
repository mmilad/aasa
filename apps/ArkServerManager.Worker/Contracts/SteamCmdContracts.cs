namespace ArkServerManager.Worker.Contracts;

public sealed record SteamCmdResult(int ExitCode, string? Output = null);

public interface ISteamCmdClient
{
    Task<SteamCmdResult> InstallOrUpdateAsync(Guid serverId, CancellationToken cancellationToken = default);
}
