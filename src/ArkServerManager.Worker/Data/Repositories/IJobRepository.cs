using ArkServerManager.Worker.Data.Entities;
using ArkServerManager.Worker.Domain;

namespace ArkServerManager.Worker.Data.Repositories;

public interface IJobRepository
{
    Task<JobEntity?> GetAsync(Guid jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads a job with change tracking so completion fields can be persisted.
    /// </summary>
    Task<JobEntity?> GetTrackedAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task<JobEntity> AddAsync(JobEntity entity, CancellationToken cancellationToken = default);
    Task<bool> HasRunningJobForServerAsync(Guid serverId, CancellationToken cancellationToken = default);
    Task<bool> HasRunningInstallAsync(CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
