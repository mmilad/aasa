namespace ArkServerManager.Worker.Infrastructure;

public static class ManagerPathsDefaults
{
    public static string GetDefaultRoot()
    {
        var envRoot = Environment.GetEnvironmentVariable("ARKMGR_DATA_ROOT");
        if (!string.IsNullOrWhiteSpace(envRoot))
        {
            return envRoot;
        }

        if (OperatingSystem.IsWindows())
        {
            var common = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            return Path.Combine(common, "ArkServerManager");
        }

        return Path.Combine(AppContext.BaseDirectory, "data");
    }
}
