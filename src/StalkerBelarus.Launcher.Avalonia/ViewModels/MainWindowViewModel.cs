using System.Diagnostics;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using StalkerBelarus.Launcher.Avalonia.Helpers;
using StalkerBelarus.Launcher.Core;
using StalkerBelarus.Launcher.Core.Manager;
using StalkerBelarus.Launcher.Core.Storage;

namespace StalkerBelarus.Launcher.Avalonia.ViewModels;

public class MainWindowViewModel : ReactiveObject, IAsyncInitialization {
    private readonly ILogger<MainWindowViewModel> _logger;

    private readonly InitializerManager _initializerManager;
    private readonly AuthorizationViewModel _authorizationViewModel;
    private readonly StartGameViewModel _startGameViewModel;
    private readonly LauncherViewModel _launcherViewModel;

    [Reactive] public ReactiveObject PageViewModel { get; set; } = null!;
    
    public Task Initialization { get; private set; }

    public MainWindowViewModel(ILogger<MainWindowViewModel> logger, InitializerManager initializerManager,
        LauncherViewModel launcherViewModel, AuthorizationViewModel authorizationViewModel,
        StartGameViewModel startGameViewModel) {
        _logger = logger;
        _initializerManager = initializerManager;

        _authorizationViewModel = authorizationViewModel;
        _startGameViewModel = startGameViewModel ?? throw new ArgumentNullException(nameof(startGameViewModel));
        _launcherViewModel = launcherViewModel ?? throw new ArgumentNullException(nameof(launcherViewModel));

        Initialization = InitializeAsync();
    }

#if DEBUG

    public MainWindowViewModel() {
        _logger = null!;
        _authorizationViewModel = null!;
        _startGameViewModel = null!;
        _launcherViewModel = null!;
        _initializerManager = null!;
        Initialization = null!;
    }

#endif

    public async Task InitializeAsync() {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        ProcessHelper.KillAllXrEngine();

        await _initializerManager.InitializeAsync();

        if (!_initializerManager.IsUserAuthorized) {
            _authorizationViewModel.SetupBinding();
        } else {
            _authorizationViewModel.UpdateNews();
        }

        var isCurrentRelease = _initializerManager.IsGameReleaseCurrent;

        if (File.Exists(FileLocations.UserSettingPath)) {
            try {
                if (!isCurrentRelease) {
                    PageViewModel = _launcherViewModel;
                    await _launcherViewModel.SelectUpdateMenuAsync();
                }
            } catch (Exception ex) {
                _logger.LogError("{Message}", ex.Message);
                _logger.LogError("{StackTrace}", ex.StackTrace);
            }
        }

        if (File.Exists(FileLocations.UserSettingPath)) {
            ShowLauncherImpl();
        } else {
            ShowAuthorizationImpl();
        }

        stopwatch.Stop();
        _logger.LogInformation("MainWindowViewModel Initialize: {Time}", stopwatch.ElapsedMilliseconds);
    }

    public void ShowLauncherImpl() {
        _launcherViewModel.SelectMenu();
        PageViewModel = _launcherViewModel;
    }

    public void ShowAuthorizationImpl() {
        PageViewModel = _authorizationViewModel;
    }

    public void ShowStartGameImpl() {
        // Update locale
        _startGameViewModel.SetupValidation();
        PageViewModel = _startGameViewModel;
    }
}
