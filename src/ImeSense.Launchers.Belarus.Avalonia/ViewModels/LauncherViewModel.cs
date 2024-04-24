using System.Reactive;

using ImeSense.Launchers.Belarus.Avalonia.Helpers;
using ImeSense.Launchers.Belarus.Avalonia.Services;
using ImeSense.Launchers.Belarus.Core.Helpers;
using ImeSense.Launchers.Belarus.Core.Services;
using ImeSense.Launchers.Belarus.Core.Storage;
using ImeSense.Launchers.Belarus.Core.Validators;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ImeSense.Launchers.Belarus.Avalonia.ViewModels;

public class LauncherViewModel : ReactiveObject
{
    private readonly ILogger<LauncherViewModel>? _logger;
    private readonly IWebsiteLauncher _websiteLauncher;
    private readonly DownloadMenuViewModel _downloadMenuViewModel;
    private readonly GameMenuViewModel _gameMenuViewModel;
    private readonly GameDirectoryValidator _directoryValidator;

    public string AppVersion { get; set; }
    public string CompanyName { get; set; }

    [Reactive] public ReactiveObject PageMenuViewModel { get; set; } = null!;
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

    public LauncherViewModel()
    {
        ExceptionHelper.ThrowIfEmptyConstructorNotInDesignTime($"{nameof(LauncherViewModel)}");

        _downloadMenuViewModel = null!;
        _gameMenuViewModel = null!;
        _directoryValidator = null!;
        _websiteLauncher = null!;

        AppVersion = null!;
        CompanyName = null!;

        NewsSliderViewModel = null!;

        OpenMainRepositoryUriCommand = null!;
        OpenOrganizationUriCommand = null!;
    }

    private void OpenUrl(string uri) => _websiteLauncher.OpenWebsite(uri);

    public void SelectMenu()
    {
        if (_directoryValidator.IsDirectoryValid()) {
            PageMenuViewModel = _gameMenuViewModel;
        } else {
            PageMenuViewModel = _downloadMenuViewModel;
        }
    }

    public async Task SelectUpdateMenuAsync()
    {
        PageMenuViewModel = _downloadMenuViewModel;
        await _downloadMenuViewModel.UpdateAsync(this);
    }
}
