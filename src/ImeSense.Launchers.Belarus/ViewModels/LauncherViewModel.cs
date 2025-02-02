using System.Reactive;

using ImeSense.Launchers.Belarus.Core.Helpers;
using ImeSense.Launchers.Belarus.Core.Services;
using ImeSense.Launchers.Belarus.Core.Storage;
using ImeSense.Launchers.Belarus.Core.Validators;
using ImeSense.Launchers.Belarus.Services;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ImeSense.Launchers.Belarus.ViewModels;

public class LauncherViewModel : ReactiveObject
{
    private readonly ILogger<LauncherViewModel>? _logger;
    private readonly IWebsiteLauncher _websiteLauncher;
    private readonly DownloadMenuViewModel _downloadMenuViewModel;
    private readonly GameMenuViewModel _gameMenuViewModel;
    private readonly GameDirectoryValidator _directoryValidator;

    public string AppVersion { get; set; }
    public string CompanyName { get; set; }

    [Reactive] public ReactiveObject? PageMenuViewModel { get; set; }
    [Reactive] public NewsSliderViewModel NewsSliderViewModel { get; set; }

    public ReactiveCommand<Unit, Unit>? OpenMainRepositoryUriCommand { get; set; }
    public ReactiveCommand<Unit, Unit>? OpenOrganizationUriCommand { get; set; }

    public LauncherViewModel(ILogger<LauncherViewModel>? logger, ViewModelLocator viewModelLocator,
        GameDirectoryValidator directoryValidator, IWebsiteLauncher websiteLauncher)
    {
        _logger = logger;
        _downloadMenuViewModel = viewModelLocator.DownloadMenuViewModel;
        _gameMenuViewModel = viewModelLocator.GameMenuViewModel;
        NewsSliderViewModel = viewModelLocator.NewsSliderViewModel;

        _directoryValidator = directoryValidator;
        _websiteLauncher = websiteLauncher;

        AppVersion = ApplicationHelper.GetAppVersion();
        CompanyName = (char) 0169 + ApplicationHelper.GetCompanyName();

        SetupCommands();
    }

    private void SetupCommands()
    {
        OpenMainRepositoryUriCommand = ReactiveCommand.Create(() => OpenUrl(UriStorage.LauncherUri.AbsoluteUri));
        OpenOrganizationUriCommand = ReactiveCommand.Create(() => OpenUrl(UriStorage.ImeSenseUri.AbsoluteUri));
    }

    private void OpenUrl(string uri) => _websiteLauncher.OpenWebsite(uri);

    public void SelectMenu()
    {
        PageMenuViewModel = _directoryValidator.IsDirectoryValid() ? _gameMenuViewModel : _downloadMenuViewModel;
    }

    public async Task SelectUpdateMenuAsync()
    {
        PageMenuViewModel = _downloadMenuViewModel;
        await _downloadMenuViewModel.UpdateAsync(this);
    }
}
