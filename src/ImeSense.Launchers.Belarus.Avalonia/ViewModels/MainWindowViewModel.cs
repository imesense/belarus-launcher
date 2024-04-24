using System.Diagnostics;

using ImeSense.Launchers.Belarus.Avalonia.Helpers;
using ImeSense.Launchers.Belarus.Avalonia.Services;
using ImeSense.Launchers.Belarus.Core.Helpers;
using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.Core.Services;
using ImeSense.Launchers.Belarus.Core.Storage;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ImeSense.Launchers.Belarus.Avalonia.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    private readonly ILogger<MainWindowViewModel>? _logger;
    private readonly IUpdaterService _updaterService;
    private readonly InitializerManager _initializerManager;
    private readonly ViewModelLocator _viewModelLocator;
    private readonly StartGameViewModel _startGameViewModel;
    private readonly LauncherViewModel _launcherViewModel;

    [Reactive] public ReactiveObject PageViewModel { get; set; } = null!;

    public MainWindowViewModel(ILogger<MainWindowViewModel>? logger, InitializerManager initializerManager,
        IUpdaterService updaterService, ViewModelLocator viewModelLocator)
    {
        _logger = logger;
        _initializerManager = initializerManager;
        _updaterService = updaterService;
        _viewModelLocator = viewModelLocator;
        _startGameViewModel = viewModelLocator.StartGameViewModel;
        _launcherViewModel = viewModelLocator.LauncherViewModel;
    }

    public MainWindowViewModel()
    {
        ExceptionHelper.ThrowIfEmptyConstructorNotInDesignTime($"{nameof(MainWindowViewModel)}");

        _startGameViewModel = null!;
        _launcherViewModel = null!;
        _initializerManager = null!;
        _updaterService = null!;
        _viewModelLocator = null!;
    }

    public async Task InitializeAsync()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        ProcessHelper.KillAllXrEngine();

        var isCurrentRelease = _initializerManager.IsGameReleaseCurrent;

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

    public void ShowSplashScreenImpl(SplashScreenViewModel splash)
    {
        PageViewModel = splash;
    }
}
