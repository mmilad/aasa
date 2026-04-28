namespace ArkServerManager.Worker.Configuration;

public sealed class ProcessSupervisorOptions
{
    public const string SectionName = "ProcessSupervisor";

    public int ShutdownGracePeriodSeconds { get; set; } = 60;

    public int KillGracePeriodSeconds { get; set; } = 5;
}
