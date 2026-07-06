namespace ArkServerManager.Worker.Configuration;

public sealed class SteamCmdOptions
{
    public const string SectionName = "SteamCmd";

    public string SteamCmdPath { get; set; } = "steamcmd.exe";

    public int InstallTimeoutSeconds { get; set; } = 1800;
}
