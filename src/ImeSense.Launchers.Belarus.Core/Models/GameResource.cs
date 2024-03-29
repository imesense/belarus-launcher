using System.Text.Json.Serialization;

namespace ImeSense.Launchers.Belarus.Core.Models;

public class GameResource {
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    [JsonPropertyName("hash")]
    public string Hash { get; set; } = string.Empty;
    [JsonPropertyName("directory")]
    public string Directory { get; set; } = string.Empty;
}
