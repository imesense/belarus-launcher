using System.Reactive;
using System.Reactive.Linq;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using ImeSense.Launchers.Belarus.Core.Helpers;
using ImeSense.Launchers.Belarus.Core.Manager;

namespace ImeSense.Launchers.Belarus.Avalonia.ViewModels;

public class GameMenuViewModel : ReactiveObject {
    private readonly ILogger<GameMenuViewModel> _logger;
    private readonly IWindowManager _windowManager;
    private readonly UserManager _userManager;
    public ReactiveCommand<MainWindowViewModel, Unit> PlayGame { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> StartServer { get; private set; } = null!;
    public ReactiveCommand<LauncherViewModel, Unit> CheckUpdates { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> Close { get; private set; } = null!;
    [Reactive] public bool IsStartServer { get; set; } = false;

    public GameMenuViewModel(ILogger<GameMenuViewModel> logger, IWindowManager windowManager, UserManager userManager) {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
        _userManager = userManager;

        SetupCommands();
    }

#if DEBUG
    public GameMenuViewModel() {
        _logger = null!;
        _windowManager = null!;
        _userManager = null!;
    }
#endif

    private void SetupCommands() {
        var canExecuteServer = this.WhenAnyValue(x => x.IsStartServer, 
                startServer => startServer == false)
            .ObserveOn(RxApp.MainThreadScheduler);
        
        PlayGame = ReactiveCommand.Create<MainWindowViewModel>(PlayGameImpl);
        StartServer = ReactiveCommand.Create(StartServerImpl, canExecuteServer);
        CheckUpdates = ReactiveCommand.CreateFromTask<LauncherViewModel>(CheckUpdatesImplAsync);
        Close = ReactiveCommand.Create(_windowManager.Close);
        
        Observable.Merge(PlayGame.ThrownExceptions, StartServer.ThrownExceptions, Close.ThrownExceptions)
            .Throttle(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler)
            .Subscribe(OnCommandException);
    }

    private async Task CheckUpdatesImplAsync(LauncherViewModel launcherViewModel) {
        await launcherViewModel.SelectUpdateMenuAsync();
    }

    private void PlayGameImpl(MainWindowViewModel mainWindowViewModel) {
        if (_userManager is null) {
            throw new NullReferenceException("User manager object is null");
        }
        if (_userManager.UserSettings is null) {
            throw new NullReferenceException("User settings object is null");
        }

        ProcessHelper.KillServers();
        
        if (IsStartServer) {
            var launch = Core.Launcher.Launch(path: @"binaries\xrEngine.exe",
                arguments: new List<string> {
                    @$"-start client(localhost/name={_userManager.UserSettings.Username})",
                    $"{_userManager.UserSettings.Locale}",
                });
            
            if (launch == null) {
                return;
            }
            launch.Start();
            
            _windowManager.Close();
        } else {
            mainWindowViewModel.ShowStartGameImpl(); 
        }
    }

    private void StartServerImpl() {
        ProcessHelper.KillAllXrEngine();
        
        var launch = Core.Launcher.Launch(path: @"binaries\xrEngine.exe",
            arguments: new List<string> {
                "-dedicated",
                "-i",
                @"-start server(belarus_lobby/fmp/timelimit=60) client(localhost)",
            });

        if (launch == null) {
            return;
        }
        launch.Exited += LaunchOnExited;
        launch.Start();
        
        IsStartServer = true;
    }

    private  void LaunchOnExited(object? sender, EventArgs e) {
        IsStartServer = false;
    }

    private void OnCommandException(Exception exception) 
        => _logger.LogError("{Message}", exception.Message);
}
