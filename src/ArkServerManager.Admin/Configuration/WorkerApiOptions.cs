namespace ArkServerManager.Admin.Configuration;

public sealed class WorkerApiOptions
{
    public const string SectionName = "Worker";

    public string BaseUrl { get; set; } = "http://127.0.0.1:5080/";

    public string ApiKey { get; set; } = "CHANGE_ME";

    public int TimeoutSeconds { get; set; } = 30;
}
