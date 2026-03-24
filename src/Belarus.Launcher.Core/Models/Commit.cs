using System.Text.Json.Serialization;

namespace Belarus.Launcher.Core.Models;

public class Commit
{
    [JsonPropertyName("sha")]
    public string Sha { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public Uri? Url { get; set; }
}
