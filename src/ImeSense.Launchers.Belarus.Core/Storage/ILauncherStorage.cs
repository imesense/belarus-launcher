using System.Collections.ObjectModel;

using ImeSense.Launchers.Belarus.Core.Models;

using ReactiveUI.Fody.Helpers;

namespace ImeSense.Launchers.Belarus.Core.Storage;

public interface ILauncherStorage
{
    GitHubRelease? GitHubRelease { get; set; }
    IList<Locale> Locales { get; }
    ObservableCollection<LangNewsContent>? NewsContents { get; set; }
    ObservableCollection<WebResource>? WebResources { get; set; }
    bool IsCheckGitHubConnection { get; set; }
    bool IsGameReleaseCurrent { get; set; }
    bool IsUserAuthorized { get; set; }
}
