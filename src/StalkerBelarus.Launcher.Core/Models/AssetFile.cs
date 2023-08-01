using System.Text.Json.Serialization;

namespace StalkerBelarus.Launcher.Core.Models;

public class AssetFile {
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    [JsonPropertyName("hash")]
    public string Hash { get; set; } = string.Empty;
}
