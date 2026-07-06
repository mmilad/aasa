using System.Text.Json.Serialization;

namespace ArkServerManager.Worker.Contracts;

public sealed record IniFileResponse(Dictionary<string, Dictionary<string, string>> Sections);

public sealed record PatchIniRequest(
    [property: JsonPropertyName("section")] string Section,
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("value")] string Value);
