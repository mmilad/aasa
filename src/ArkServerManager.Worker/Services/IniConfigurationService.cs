using ArkServerManager.Worker.Contracts;
using ArkServerManager.Worker.Infrastructure;

namespace ArkServerManager.Worker.Services;

public interface IIniConfigurationService
{
    Task<IniFileResponse?> ReadGameUserSettingsAsync(Guid serverId, CancellationToken cancellationToken = default);
    Task<IniFileResponse?> ReadGameIniAsync(Guid serverId, CancellationToken cancellationToken = default);
    Task<(IniFileResponse? Response, ApiError? Error)> PatchGameUserSettingsAsync(
        Guid serverId,
        PatchIniRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class IniConfigurationService(IDataDirectoryService dataDirectoryService) : IIniConfigurationService
{
    private static readonly Dictionary<string, HashSet<string>> AllowList = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ServerSettings"] = new(StringComparer.OrdinalIgnoreCase)
        {
            "ServerPassword",
            "ServerAdminPassword",
            "MaxPlayers",
        },
    };

    public async Task<IniFileResponse?> ReadGameUserSettingsAsync(Guid serverId, CancellationToken cancellationToken = default)
    {
        var path = GetGameUserSettingsPath(serverId);
        if (!File.Exists(path))
        {
            return null;
        }

        var text = await File.ReadAllTextAsync(path, cancellationToken);
        return new IniFileResponse(ParseIni(text));
    }

    public async Task<IniFileResponse?> ReadGameIniAsync(Guid serverId, CancellationToken cancellationToken = default)
    {
        var path = GetGameIniPath(serverId);
        if (!File.Exists(path))
        {
            return null;
        }

        var text = await File.ReadAllTextAsync(path, cancellationToken);
        return new IniFileResponse(ParseIni(text));
    }

    public async Task<(IniFileResponse? Response, ApiError? Error)> PatchGameUserSettingsAsync(
        Guid serverId,
        PatchIniRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!AllowList.TryGetValue(request.Section, out var keys) || !keys.Contains(request.Key))
        {
            return (
                null,
                new ApiError(
                    "KEY_NOT_ALLOWLISTED",
                    "The requested INI key is not allowed in MVP.",
                    new { request.Section, request.Key }));
        }

        var path = GetGameUserSettingsPath(serverId);
        if (!File.Exists(path))
        {
            return (null, new ApiError("NOT_FOUND", "GameUserSettings.ini not found for this server."));
        }

        var text = await File.ReadAllTextAsync(path, cancellationToken);
        var parsed = ParseIni(text);
        if (!parsed.TryGetValue(request.Section, out var section))
        {
            section = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            parsed[request.Section] = section;
        }

        section[request.Key] = request.Value;
        var backupPath = $"{path}.bak.{DateTime.UtcNow:yyyyMMddHHmmss}";
        File.Copy(path, backupPath, overwrite: true);
        await File.WriteAllTextAsync(path, SerializeIni(parsed), cancellationToken);
        return (new IniFileResponse(parsed), null);
    }

    private string GetGameUserSettingsPath(Guid serverId)
    {
        return Path.Combine(
            dataDirectoryService.GetServerBinariesRoot(serverId),
            "ShooterGame",
            "Saved",
            "Config",
            "WindowsServer",
            "GameUserSettings.ini");
    }

    private string GetGameIniPath(Guid serverId)
    {
        return Path.Combine(
            dataDirectoryService.GetServerBinariesRoot(serverId),
            "ShooterGame",
            "Saved",
            "Config",
            "WindowsServer",
            "Game.ini");
    }

    private static Dictionary<string, Dictionary<string, string>> ParseIni(string content)
    {
        var result = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
        var currentSection = "Default";

        foreach (var rawLine in content.Split('\n', StringSplitOptions.None))
        {
            var line = rawLine.Trim();
            if (line.Length == 0 || line.StartsWith(';') || line.StartsWith('#'))
            {
                continue;
            }

            if (line.StartsWith('[') && line.EndsWith(']'))
            {
                currentSection = line[1..^1].Trim();
                if (!result.ContainsKey(currentSection))
                {
                    result[currentSection] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }

                continue;
            }

            var split = line.IndexOf('=');
            if (split <= 0)
            {
                continue;
            }

            if (!result.TryGetValue(currentSection, out var section))
            {
                section = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                result[currentSection] = section;
            }

            var key = line[..split].Trim();
            var value = line[(split + 1)..].Trim();
            section[key] = value;
        }

        return result;
    }

    private static string SerializeIni(Dictionary<string, Dictionary<string, string>> sections)
    {
        var lines = new List<string>();
        foreach (var section in sections)
        {
            lines.Add($"[{section.Key}]");
            foreach (var kvp in section.Value)
            {
                lines.Add($"{kvp.Key}={kvp.Value}");
            }

            lines.Add(string.Empty);
        }

        return string.Join(Environment.NewLine, lines);
    }
}
