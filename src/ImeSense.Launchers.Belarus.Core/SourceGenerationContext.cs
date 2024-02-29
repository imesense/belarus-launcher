using System.Text.Json.Serialization;

using ImeSense.Launchers.Belarus.Core.Models;

namespace ImeSense.Launchers.Belarus.Core;

[JsonSourceGenerationOptions(WriteIndented = true, AllowTrailingCommas = true)]
[JsonSerializable(typeof(Asset))]
[JsonSerializable(typeof(Commit))]
[JsonSerializable(typeof(GameResource))]
[JsonSerializable(typeof(GameResource[]))]
[JsonSerializable(typeof(GitHubRelease))]
[JsonSerializable(typeof(GitHubRelease[]))]
[JsonSerializable(typeof(Locale))]
[JsonSerializable(typeof(NewsContent))]
[JsonSerializable(typeof(IEnumerable<NewsContent>))]
[JsonSerializable(typeof(IEnumerable<LangNewsContent>))]
[JsonSerializable(typeof(Tag))]
[JsonSerializable(typeof(IEnumerable<Tag>))]
[JsonSerializable(typeof(UserSettings))]
[JsonSerializable(typeof(WebResource))]
[JsonSerializable(typeof(WebResource[]))]
[JsonSerializable(typeof(IEnumerable<WebResource>))]
public partial class SourceGenerationContext : JsonSerializerContext {
}
