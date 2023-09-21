using System.Reactive;
using System.Reactive.Linq;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using StalkerBelarus.Launcher.Avalonia.Helpers;
using StalkerBelarus.Launcher.Core.Manager;
using StalkerBelarus.Launcher.Core.Models;

namespace StalkerBelarus.Launcher.Avalonia.ViewModels;

public class GameMenuViewModel : ViewModelBase {
    private readonly ILogger<GameMenuViewModel> _logger;
    private readonly IWindowManager _windowManager;
    private readonly UserSettings _userSettings;
    public ReactiveCommand<MainWindowViewModel, Unit> PlayGame { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> StartServer { get; private set; } = null!;
    public ReactiveCommand<LauncherViewModel, Unit> CheckUpdates { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> Close { get; private set; } = null!;
    [Reactive] public bool IsStartServer { get; set; } = false;

    public GameMenuViewModel(ILogger<GameMenuViewModel> logger, IWindowManager windowManager, 
        UserSettings userSettings) {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
        _userSettings = userSettings ?? throw new ArgumentNullException(nameof(userSettings));
        
        SetupCommands();
    }

#if DEBUG
    public GameMenuViewModel() {
        _logger = null!;
        _windowManager = null!;
        _userSettings = null!;
    }
#endif

    private void SetupCommands() {
        var canExecuteServer = this.WhenAnyValue(x => x.IsStartServer, 
                startServer => startServer == false)
            .ObserveOn(RxApp.MainThreadScheduler);
        
        PlayGame = ReactiveCommand.Create<MainWindowViewModel>(PlayGameImpl);
        StartServer = ReactiveCommand.Create(StartServerImpl, canExecuteServer);
        CheckUpdates = ReactiveCommand.Create<LauncherViewModel>(CheckUpdatesImpl);
        Close = ReactiveCommand.Create(_windowManager.Close);
        
        Observable.Merge(PlayGame.ThrownExceptions, StartServer.ThrownExceptions, Close.ThrownExceptions)
            .Throttle(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler)
            .Subscribe(OnCommandException);
    }

    private void CheckUpdatesImpl(LauncherViewModel launcherViewModel) {
        launcherViewModel.SelectUpdateMenu();
    }

    private void PlayGameImpl(MainWindowViewModel mainWindowViewModel) {
        ProcessHelper.KillServers();
        
        if (IsStartServer) {
            var launch = Core.Launcher.Launch(path: @"binaries\xrEngine.exe",
                arguments: new List<string> {
                    @$"-start client(localhost/name={_userSettings.Username})",
                    $"{_userSettings.Locale}",
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
