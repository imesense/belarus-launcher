using System.Text.Json.Serialization;

namespace ImeSense.Launchers.Belarus.Core.Models;

public class Asset {
    [JsonPropertyName("url")]
    public Uri? Url { get; set; }
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("node_id")]
    public string NodeId { get; set; } = null!;
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    [JsonPropertyName("label")]
    public object? Label { get; set; }
    [JsonPropertyName("content_type")]
    public string ContentType { get; set; } = null!;
    [JsonPropertyName("state")]
    public string State { get; set; } = null!;
    [JsonPropertyName("size")]
    public int Size { get; set; }
    //[JsonPropertyName("download_count")]
    //public int DownloadCount { get; set; }
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
    [JsonPropertyName("browser_download_url")]
    public Uri? BrowserDownloadUrl { get; set; }
}
