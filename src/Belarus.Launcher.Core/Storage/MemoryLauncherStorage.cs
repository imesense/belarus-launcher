using System.Collections.ObjectModel;

using Belarus.Launcher.Core.Models;

using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace Belarus.Launcher.Core.Storage;

public partial class MemoryLauncherStorage : ReactiveObject, ILauncherStorage
{
    public GitHubRelease? GitHubRelease { get; set; }

    public IList<Locale> Locales { get; } =
    [
        new() { Key = "rus", Title = "Русский", },
        // new() { Key = "be", Title = "Беларуская", },
        new() { Key = "eng", Title = "English", },
    ];

    [Reactive] public partial ObservableCollection<LangNewsContent>? NewsContents { get; set; }
    [Reactive] public partial ObservableCollection<WebResource>? WebResources { get; set; }
    [Reactive] public partial bool IsCheckGitHubConnection { get; set; }
    [Reactive] public partial bool IsGameReleaseCurrent { get; set; } = true;
    [Reactive] public partial bool IsUserAuthorized { get; set; }
}
