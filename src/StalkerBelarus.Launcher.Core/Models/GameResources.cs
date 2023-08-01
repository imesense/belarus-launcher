using System.Text.Json.Serialization;

namespace StalkerBelarus.Launcher.Core.Models;

public class GameResources
{
    [JsonPropertyName("binaries")]
    public IEnumerable<AssetFile>? Binaries { get; set; }
    [JsonPropertyName("resources")]
    public IEnumerable<AssetFile>? Resources { get; set; }
    [JsonPropertyName("patches")]
    public IEnumerable<AssetFile>? Patches { get; set; }
}
