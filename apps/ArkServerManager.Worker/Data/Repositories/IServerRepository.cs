using ArkServerManager.Worker.Data.Entities;

namespace ArkServerManager.Worker.Data.Repositories;

public interface IServerRepository
{
    Task<IReadOnlyList<ServerEntity>> ListAsync(CancellationToken cancellationToken = default);
    Task<ServerEntity?> GetAsync(Guid serverId, CancellationToken cancellationToken = default);
    Task<ServerEntity> AddAsync(ServerEntity entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid serverId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
