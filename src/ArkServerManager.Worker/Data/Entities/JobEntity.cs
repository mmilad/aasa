using ArkServerManager.Worker.Domain;

namespace ArkServerManager.Worker.Data.Entities;

public sealed class JobEntity
{
    public Guid Id { get; set; }
    public Guid ServerId { get; set; }
    public JobType Type { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Pending;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? FinishedAtUtc { get; set; }
    public int? ExitCode { get; set; }
    public string? LogBlob { get; set; }

    public ServerEntity Server { get; set; } = null!;
}
