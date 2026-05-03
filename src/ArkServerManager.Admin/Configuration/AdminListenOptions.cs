namespace ArkServerManager.Admin.Configuration;

public sealed class AdminListenOptions
{
    public const string SectionName = "Listen";

    public bool Public { get; set; }

    public int Port { get; set; } = 5081;
}
