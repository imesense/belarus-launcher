using System.Collections.ObjectModel;

using ImeSense.Launchers.Belarus.Core.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ImeSense.Launchers.Belarus.Core.Storage;

public class MemoryLauncherStorage : ReactiveObject, ILauncherStorage
{
    public GitHubRelease? GitHubRelease { get; set; }

    public IList<Locale> Locales { get; } = new List<Locale> {
            new() { Key = "rus", Title = "Русский", },
            // new() { Key = "be", Title = "Беларуская", },
            new() { Key = "eng", Title = "English", },
        };

    [Reactive]
    public ObservableCollection<LangNewsContent>? NewsContents { get; set; }
    [Reactive]
    public ObservableCollection<WebResource>? WebResources { get; set; }
    [Reactive]
    public bool IsCheckGitHubConnection { get; set; }
}
