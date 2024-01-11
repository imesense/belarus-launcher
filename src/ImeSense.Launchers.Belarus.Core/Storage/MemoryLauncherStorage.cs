using ImeSense.Launchers.Belarus.Core.Models;

namespace ImeSense.Launchers.Belarus.Core.Storage;

public class MemoryLauncherStorage : ILauncherStorage {
    public GitHubRelease? GitHubRelease { get; set; }

    public IList<Locale> Locales { get; } = new List<Locale> {
            new() { Key = "rus", Title = "Русский", },
            // new() { Key = "be", Title = "Беларуская", },
            new() { Key = "eng", Title = "English", },
        };

    public IEnumerable<LangNewsContent>? NewsContents { get; set; }

    public IEnumerable<WebResource>? WebResources { get; set; }
}
