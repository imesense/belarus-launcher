using System.Collections.ObjectModel;
using System.Reactive;

using ReactiveUI;

using StalkerBelarus.Launcher.Core;
using StalkerBelarus.Launcher.Core.Models;
using StalkerBelarus.Launcher.Core.Services;

namespace StalkerBelarus.Launcher.Avalonia.ViewModels; 

public class LinkViewModel : ViewModelBase, IAsyncInitialization {
    private readonly IWebsiteLauncher _websiteLauncher;
    private readonly IGitHubApiService _gitHubApiService;
    public ObservableCollection<WebResource>? WebResources { get; set; } = new();
    public ReactiveCommand<string, Unit> OpenUrlCommand { get; set; }
    public Task Initialization { get; }

    public LinkViewModel(IWebsiteLauncher websiteLauncher, IGitHubApiService gitHubApiService) {
        _websiteLauncher = websiteLauncher;
        _gitHubApiService = gitHubApiService;
        Initialization = InitializeAsync();
        OpenUrlCommand = ReactiveCommand.Create<string>(OpenUrl);
    }
    
    private async Task InitializeAsync() {
        // Asynchronously initialize this instance.
        await LoadWebResources();
    }
    
    private async Task LoadWebResources() {
        var contents = _gitHubApiService.DownloadJsonArrayAsync<WebResource>("WebResources.json");
        await foreach (var content in contents) {
            if (content != null) {
                WebResources?.Add(content);
            }
        }
    }

    private void OpenUrl(string url) => _websiteLauncher.OpenWebsite(url);
}
