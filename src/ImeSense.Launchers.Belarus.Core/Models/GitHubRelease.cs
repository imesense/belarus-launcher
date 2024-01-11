using System.Text.Json.Serialization;

namespace ImeSense.Launchers.Belarus.Core.Models;

public class GitHubRelease {
    [JsonPropertyName("url")]
    public Uri? Url { get; set; }
    [JsonPropertyName("assets_url")]
    public Uri? AssetsUrl { get; set; }
    [JsonPropertyName("upload_url")]
    public Uri? UploadUrl { get; set; }
    [JsonPropertyName("html_url")]
    public Uri? HtmlUrl { get; set; }
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("node_id")]
    public string NodeId { get; set; } = null!;
    [JsonPropertyName("tag_name")]
    public string TagName { get; set; } = null!;
    [JsonPropertyName("target_commitish")]
    public string? TargetCommitish { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    [JsonPropertyName("draft")]
    public bool Draft { get; set; }
    [JsonPropertyName("prerelease")]
    public bool PreRelease { get; set; }
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    [JsonPropertyName("published_at")]
    public DateTime PublishedAt { get; set; }
    [JsonPropertyName("assets")]
    public IEnumerable<Asset>? Assets { get; set; }
    [JsonPropertyName("tarball_url")]
    public Uri? TarballUrl { get; set; }
    [JsonPropertyName("zipball_url")]
    public Uri? ZipballUrl { get; set; }
    [JsonPropertyName("body")]
    public string Body { get; set; } = null!;
}
