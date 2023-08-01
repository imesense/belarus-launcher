using System.Text.Json.Serialization;

namespace StalkerBelarus.Launcher.Core.Models; 

public sealed class WebResource {
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}
