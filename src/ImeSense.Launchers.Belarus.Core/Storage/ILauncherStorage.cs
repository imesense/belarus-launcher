using ImeSense.Launchers.Belarus.Core.Models;

using ReactiveUI.Fody.Helpers;

namespace ImeSense.Launchers.Belarus.Core.Storage;

public interface ILauncherStorage
{
    GitHubRelease? GitHubRelease { get; set; }
    IList<Locale> Locales { get; }
    IList<LangNewsContent>? NewsContents { get; set; }
    IEnumerable<WebResource>? WebResources { get; set; }
    [Reactive]
    bool IsCheckGitHubConnection { get; set; }
}
