using System.Text.Json.Serialization;

namespace StalkerBelarus.Launcher.Core.Models;

public class GitHubRelease {
    [JsonPropertyName("url")]
    public string Url { get; set; } = null!;
    [JsonPropertyName("assets_url")]
    public string? AssetsUrl { get; set; }
    [JsonPropertyName("upload_url")]
    public string UploadUrl { get; set; } = null!;
    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = null!;
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
    public string? TarballUrl { get; set; }
    [JsonPropertyName("zipball_url")]
    public string? ZipballUrl { get; set; }
    [JsonPropertyName("body")]
    public string Body { get; set; } = null!;
}
