using ArkServerManager.Worker.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArkServerManager.Worker.Data.Repositories;

public sealed class ServerRepository(AppDbContext dbContext) : IServerRepository
{
    public async Task<IReadOnlyList<ServerEntity>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Servers
            .OrderBy(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<ServerEntity?> GetAsync(Guid serverId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Servers
            .FirstOrDefaultAsync(x => x.Id == serverId, cancellationToken);
    }

    public async Task<ServerEntity> AddAsync(ServerEntity entity, CancellationToken cancellationToken = default)
    {
        var created = await dbContext.Servers.AddAsync(entity, cancellationToken);
        return created.Entity;
    }

    public async Task<bool> DeleteAsync(Guid serverId, CancellationToken cancellationToken = default)
    {
        var entity = await GetAsync(serverId, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        dbContext.Servers.Remove(entity);
        return true;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
