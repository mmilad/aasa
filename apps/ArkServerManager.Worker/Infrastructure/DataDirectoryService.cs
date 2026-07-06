using ArkServerManager.Worker.Configuration;
using Microsoft.Extensions.Options;

namespace ArkServerManager.Worker.Infrastructure;

public interface IDataDirectoryService
{
    string ManagerDataRoot { get; }
    string GetServerRoot(Guid serverId);
    string GetServerBinariesRoot(Guid serverId);
    Task EnsureServerTreeAsync(Guid serverId, CancellationToken cancellationToken = default);
}

public sealed class DataDirectoryService(IOptions<ManagerPathsOptions> options) : IDataDirectoryService
{
    public string ManagerDataRoot => options.Value.ManagerDataRoot;

    public string GetServerRoot(Guid serverId)
    {
        return Path.Combine(ManagerDataRoot, "servers", serverId.ToString("D").ToLowerInvariant());
    }

    public string GetServerBinariesRoot(Guid serverId)
    {
        return Path.Combine(GetServerRoot(serverId), "binaries");
    }

    public Task EnsureServerTreeAsync(Guid serverId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var root = GetServerRoot(serverId);
        Directory.CreateDirectory(root);
        Directory.CreateDirectory(Path.Combine(root, "binaries"));
        Directory.CreateDirectory(Path.Combine(root, "configs"));
        Directory.CreateDirectory(Path.Combine(root, "mods"));
        Directory.CreateDirectory(Path.Combine(root, "logs"));
        Directory.CreateDirectory(Path.Combine(root, "backups"));
        return Task.CompletedTask;
    }
}
