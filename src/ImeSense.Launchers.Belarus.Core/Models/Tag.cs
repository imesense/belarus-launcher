using System.Text.Json.Serialization;

namespace ImeSense.Launchers.Belarus.Core.Models;

public class Tag {
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("zipball_url")]
    public Uri? Zipball { get; set; }
    [JsonPropertyName("tarball_url")]
    public Uri? Tarball { get; set; }
    [JsonPropertyName("commit")]
    public Commit? Commit { get; set; }
    [JsonPropertyName("node_id")]
    public string NodeId { get; set; } = string.Empty;
}
