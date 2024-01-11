using System.Text.Json.Serialization;

namespace ImeSense.Launchers.Belarus.Core.Models;

public sealed class NewsContent {
    [JsonPropertyName("title")]
    public string Title { get; init; }
    [JsonPropertyName("description")]
    public string Description { get; init; }

    public NewsContent(string title, string description) {
        Title = title;
        Description = description;
    }
}
