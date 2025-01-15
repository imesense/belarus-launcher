using System.Diagnostics;

using ImeSense.Launchers.Belarus.Services;
using ImeSense.Launchers.Belarus.Core.Helpers;
using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.Core.Services;
using ImeSense.Launchers.Belarus.Core.Storage;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ImeSense.Launchers.Belarus.Models;

namespace ImeSense.Launchers.Belarus.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    private readonly ILogger<MainWindowViewModel>? _logger;
    private readonly ILauncherStorage _launcherStorage;
    private readonly IUpdaterService _updaterService;
    private readonly ViewModelLocator _viewModelLocator;
    private readonly IApplicationLocaleManager _localeManager;
    private readonly StartGameViewModel _startGameViewModel;
    private readonly LauncherViewModel _launcherViewModel;
    private readonly SplashScreenViewModel _splashScreenViewModel;

    [Reactive] public ReactiveObject PageViewModel { get; set; } = null!;

    public MainWindowViewModel(ILogger<MainWindowViewModel>? logger, ILauncherStorage launcherStorage,
        IUpdaterService updaterService, ViewModelLocator viewModelLocator, IApplicationLocaleManager localeManager)
    {
        _logger = logger;
        _launcherStorage = launcherStorage;
        _updaterService = updaterService;
        _viewModelLocator = viewModelLocator;
        _localeManager = localeManager;
        _startGameViewModel = viewModelLocator.StartGameViewModel;
        _launcherViewModel = viewModelLocator.LauncherViewModel;
        _splashScreenViewModel = _viewModelLocator.SplashScreenViewModel;
    }

    public async Task InitializeAsync(ISplashScreenManager splashScreenManager)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        splashScreenManager.UpdateInformation(new InformationMessage(
            _localeManager.GetStringByKey("LocalizedStrings.Loading"),
            _localeManager.GetStringByKey("LocalizedStrings.DataInitialization")));

        ProcessHelper.KillAllXrEngine();

        var isCurrentRelease = _launcherStorage.IsGameReleaseCurrent;
        if (File.Exists(PathStorage.LauncherSetting)) {
            try {
                if (!isCurrentRelease) {
                    PageViewModel = _launcherViewModel;
                    await _launcherViewModel.SelectUpdateMenuAsync();
                }
            } catch (Exception ex) {
                _logger?.LogError("{Message}", ex.Message);
                _logger?.LogError("{StackTrace}", ex.StackTrace);
            }
        }

        if (File.Exists(PathStorage.LauncherSetting)) {
            _launcherViewModel.SelectMenu();
            ShowLauncherImpl();
        } else {
            ShowAuthorizationImpl();
        }

        stopwatch.Stop();
        _logger?.LogInformation("MainWindowViewModel Initialize: {Time}", stopwatch.ElapsedMilliseconds);
    }

    public void ShowLauncherImpl()
    {
        PageViewModel = _launcherViewModel;
    }

    public void ShowAuthorizationImpl()
    {
        PageViewModel = _viewModelLocator.AuthorizationViewModel;
    }

    public void ShowStartGameImpl()
    {
        PageViewModel = _startGameViewModel;
    }

    public void ShowSplashScreenImpl()
    {
        PageViewModel = _splashScreenViewModel;
    }
}
