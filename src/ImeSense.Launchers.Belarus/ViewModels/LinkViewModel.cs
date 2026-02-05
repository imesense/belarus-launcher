using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;

using DynamicData;

using ImeSense.Launchers.Belarus.Core.Models;
using ImeSense.Launchers.Belarus.Core.Services;
using ImeSense.Launchers.Belarus.Core.Storage;

using Microsoft.Extensions.Logging;

using ReactiveUI;

namespace ImeSense.Launchers.Belarus.ViewModels;

public partial class LinkViewModel : ReactiveObject
{
    private readonly ILogger<LinkViewModel>? _logger;
    private readonly IWebsiteLauncher _websiteLauncher;
    private readonly ILauncherStorage _launcherStorage;

    public ObservableCollection<WebResource> WebResources { get; set; } = [];
    public ReactiveCommand<string, Unit> OpenUrlCommand { get; set; }

    public LinkViewModel(ILogger<LinkViewModel>? logger, IWebsiteLauncher websiteLauncher,
        ILauncherStorage launcherStorage)
    {
        _logger = logger;
        _websiteLauncher = websiteLauncher;
        _launcherStorage = launcherStorage;

        OpenUrlCommand = ReactiveCommand.Create<string>(OpenUrl);

        this.WhenAnyValue(x => x._launcherStorage.WebResources)
            .Where(webRes => webRes != null && webRes.Any())
            .Subscribe((n) => Init());
    }

    private void Init()
    {
        if (_launcherStorage.WebResources is not null)
        {
            _logger?.LogInformation("Web resources are initialized");
            WebResources.AddRange(_launcherStorage.WebResources);
        }
        else
        {
            _logger?.LogError("Web resources is null!");
        }
    }

    private void OpenUrl(string url) => _websiteLauncher.OpenWebsite(url);
}
