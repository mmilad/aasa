using ArkServerManager.Worker.Domain;

namespace ArkServerManager.Worker.Data.Entities;

public sealed class ServerEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public ServerState State { get; set; } = ServerState.Stopped;
    public string InstallRoot { get; set; } = string.Empty;
    public int GamePort { get; set; } = 7777;
    public int QueryPort { get; set; } = 27015;
    public int RconPort { get; set; } = 27020;
    public string RconPassword { get; set; } = string.Empty;
    public string MapName { get; set; } = "TheIsland_WP";
    public string SessionName { get; set; } = string.Empty;
    public int? Pid { get; set; }
    public Guid? LastJobId { get; set; }

    public List<JobEntity> Jobs { get; set; } = new();
}
