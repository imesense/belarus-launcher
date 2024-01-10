using StalkerBelarus.Launcher.Core.Models;

namespace StalkerBelarus.Launcher.Core.Storage;

public interface ILauncherStorage {
    GitHubRelease? GitHubRelease { get; set; }
    IList<Locale> Locales { get; }
    IEnumerable<LangNewsContent>? NewsContents { get; set; }
    IEnumerable<WebResource>? WebResources { get; set; }
}

public class MemoryLauncherStorage : ILauncherStorage {
    public GitHubRelease? GitHubRelease { get; set; }

    public IList<Locale> Locales { get; } = new List<Locale> {
            new() { Key = "ru", Title = "Русский", },
            // new() { Key = "be", Title = "Беларуская", },
            new() { Key = "en", Title = "English", },
        };

    public IEnumerable<LangNewsContent>? NewsContents { get; set; }

    public IEnumerable<WebResource>? WebResources { get; set; }
}
