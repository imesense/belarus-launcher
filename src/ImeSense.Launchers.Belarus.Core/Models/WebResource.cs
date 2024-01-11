using System.Text.Json.Serialization;

namespace ImeSense.Launchers.Belarus.Core.Models; 

public sealed class WebResource {
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}
