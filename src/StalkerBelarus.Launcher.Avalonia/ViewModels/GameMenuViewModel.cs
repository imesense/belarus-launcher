using System.Reactive;
using System.Reactive.Linq;

using Microsoft.Extensions.Logging;

using ReactiveUI;

using StalkerBelarus.Launcher.Core.Manager;
using StalkerBelarus.Launcher.Core.Models;

namespace StalkerBelarus.Launcher.Avalonia.ViewModels;

public class GameMenuViewModel : ViewModelBase {
    private readonly ILogger<GameMenuViewModel> _logger;
    private readonly IWindowManager _windowManager;
    private readonly UserSettings _userSettings;
    private bool _isStartServer = false;
    public ReactiveCommand<MainWindowViewModel, Unit> PlayGame { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> StartServer { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> Close { get; private set; } = null!;
    
    public GameMenuViewModel(ILogger<GameMenuViewModel> logger, IWindowManager windowManager, 
        UserSettings userSettings) {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
        _userSettings = userSettings ?? throw new ArgumentNullException(nameof(userSettings));
        SetupCommands();
    }

    private void SetupCommands() {
        PlayGame = ReactiveCommand.Create<MainWindowViewModel>(PlayGameImpl);
        StartServer = ReactiveCommand.Create(StartServerImpl);
        Close = ReactiveCommand.Create(_windowManager.Close);
        
        Observable.Merge(PlayGame.ThrownExceptions, StartServer.ThrownExceptions, Close.ThrownExceptions)
            .Throttle(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler)
            .Subscribe(OnCommandException);
    }
    
    private void PlayGameImpl(MainWindowViewModel mainWindowViewModel) {
        if (_isStartServer) {
            Core.Launcher.Launch(path: @"binaries\xrEngine.exe",
                arguments: new List<string> {
                    @$"-start client(localhost/name={_userSettings.Username})"
                });
            _windowManager.Close();
            return;
        }
        
        mainWindowViewModel.ShowStartGameImpl();
    }

    private void StartServerImpl() {
        Core.Launcher.Launch(path: @"binaries\xrEngine.exe",
            arguments: new List<string> {
                "-dedicated",
                "-i",
                @"-start server(belarus_lobby/fmp/timelimit=60) client(localhost)",
            });
        _isStartServer = true;
    }
    
    private void OnCommandException(Exception exception) 
        => _logger.LogError("{Message}", exception.Message);
}
