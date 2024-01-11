using ImeSense.Launchers.Belarus.Core.Models;

namespace ImeSense.Launchers.Belarus.Core.Storage;

public interface ILauncherStorage {
    GitHubRelease? GitHubRelease { get; set; }
    IList<Locale> Locales { get; }
    IEnumerable<LangNewsContent>? NewsContents { get; set; }
    IEnumerable<WebResource>? WebResources { get; set; }
}
