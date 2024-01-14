using System.Collections.ObjectModel;
using System.Reactive;

using DynamicData;

using Microsoft.Extensions.Logging;

using ReactiveUI;

using ImeSense.Launchers.Belarus.Avalonia.Helpers;
using ImeSense.Launchers.Belarus.Core.Models;
using ImeSense.Launchers.Belarus.Core.Services;
using ImeSense.Launchers.Belarus.Core.Storage;

namespace ImeSense.Launchers.Belarus.Avalonia.ViewModels;

public class LinkViewModel : ReactiveObject {
    private readonly ILogger<LinkViewModel> _logger;
    private readonly IWebsiteLauncher _websiteLauncher;
    private readonly ILauncherStorage _launcherStorage;

    public ObservableCollection<WebResource> WebResources { get; set; } = new();
    public ReactiveCommand<string, Unit> OpenUrlCommand { get; set; }

    public LinkViewModel(ILogger<LinkViewModel> logger, IWebsiteLauncher websiteLauncher,
        ILauncherStorage launcherStorage) {
        _logger = logger;
        _websiteLauncher = websiteLauncher;
        _launcherStorage = launcherStorage;

        OpenUrlCommand = ReactiveCommand.Create<string>(OpenUrl);
    }

    public LinkViewModel() {
        ExceptionHelper.ThrowIfEmptyConstructorNotInDesignTime($"{nameof(LinkViewModel)}");

        _websiteLauncher = null!;
        OpenUrlCommand = null!;
        _logger = null!;
        _launcherStorage = null!;
    }

    public void Init() {
        if (_launcherStorage.WebResources != null) {
            _logger.LogInformation("Web resources are initialized");
            WebResources.AddRange(_launcherStorage.WebResources);
        } else {
            _logger.LogError("Web resources is null!");
        }
    }

    private void OpenUrl(string url) => _websiteLauncher.OpenWebsite(url);
}
