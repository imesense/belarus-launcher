using System.Diagnostics;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using ImeSense.Launchers.Belarus.Core;
using ImeSense.Launchers.Belarus.Core.Helpers;
using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.Core.Storage;
using ImeSense.Launchers.Belarus.Core.Services;

namespace ImeSense.Launchers.Belarus.Avalonia.ViewModels;

public class MainWindowViewModel : ReactiveObject, IAsyncInitialization {
    private readonly ILogger<MainWindowViewModel> _logger;

    private readonly InitializerManager _initializerManager;
    private readonly IUpdaterService _updaterService;
    private readonly AuthorizationViewModel _authorizationViewModel;
    private readonly StartGameViewModel _startGameViewModel;
    private readonly LauncherViewModel _launcherViewModel;

    [Reactive] public ReactiveObject PageViewModel { get; set; } = null!;
    
    public Task Initialization { get; private set; }

    public MainWindowViewModel(ILogger<MainWindowViewModel> logger, InitializerManager initializerManager,
        IUpdaterService updaterService, LauncherViewModel launcherViewModel,
        AuthorizationViewModel authorizationViewModel, StartGameViewModel startGameViewModel) {
        _logger = logger;
        _initializerManager = initializerManager;
        _updaterService = updaterService;
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
        _updaterService = null!;
        Initialization = null!;
    }

#endif

    public async Task InitializeAsync() {
        try {
            var isLauncherReleaseCurrent = await _initializerManager.IsLauncherReleaseCurrentAsync();
            if (!isLauncherReleaseCurrent) {
                var pathLauncherUpdater = Path.Combine(FileLocations.BaseDirectory,
                    FileNamesStorage.SBLauncherUpdater);
                await _updaterService.UpdaterAsync(UriStorage.LauncherUri, pathLauncherUpdater);

                var updater = Launcher.Launch(pathLauncherUpdater);
                updater?.Start();

                return;
            }
        } catch (Exception ex) {
            _logger.LogError("{Message}", ex.Message);
            _logger.LogError("{StackTrace}", ex.StackTrace);
        }

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
