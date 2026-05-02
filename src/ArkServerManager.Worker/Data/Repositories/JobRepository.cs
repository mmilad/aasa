using ArkServerManager.Worker.Data.Entities;
using ArkServerManager.Worker.Domain;
using Microsoft.EntityFrameworkCore;

namespace ArkServerManager.Worker.Data.Repositories;

public sealed class JobRepository(AppDbContext dbContext) : IJobRepository
{
    public async Task<JobEntity?> GetAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Jobs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == jobId, cancellationToken);
    }

    public async Task<JobEntity?> GetTrackedAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Jobs.FirstOrDefaultAsync(x => x.Id == jobId, cancellationToken);
    }

    public async Task<JobEntity> AddAsync(JobEntity entity, CancellationToken cancellationToken = default)
    {
        var created = await dbContext.Jobs.AddAsync(entity, cancellationToken);
        return created.Entity;
    }

    public async Task<bool> HasRunningJobForServerAsync(Guid serverId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Jobs
            .AnyAsync(
                x => x.ServerId == serverId && x.Status == JobStatus.Running,
                cancellationToken);
    }

    public async Task<bool> HasRunningInstallAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Jobs
            .AnyAsync(
                x => x.Type == JobType.InstallOrUpdate && x.Status == JobStatus.Running,
                cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
