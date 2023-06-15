using System.Reactive.Linq;

using ReactiveUI.Fody.Helpers;

using StalkerBelarus.Launcher.ViewModels.Manager;

namespace StalkerBelarus.Launcher.ViewModels;

public class MenuViewModel : ReactiveObject {
    private readonly IWindowManager _windowManager;
    private readonly UserSettings _userSettings;
    public LauncherViewModel LauncherViewModel { get; set; }
    private bool _isStartServer = false;

    [Reactive] public bool IsVisibleDownload { get; set; } = true;
    [Reactive] public bool IsVisiblePlayGame { get; set; } = false ;

    public ReactiveCommand<Unit, Unit> Close { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> PlayGame { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> StartServer { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> CheckUpdates { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> StartDownload { get; private set; } = null!;

    public MenuViewModel(IWindowManager windowManager, UserSettings userSettings) {
        _windowManager = windowManager;
        _userSettings = userSettings;

        SetupCommands();
    }

    private void SetupCommands() {
        Close = ReactiveCommand.Create(_windowManager.Close);
        PlayGame = ReactiveCommand.Create(() => PlayGameImpl());
        StartServer = ReactiveCommand.CreateFromObservable(StartServerImpl);
        CheckUpdates = ReactiveCommand.CreateFromTask(CheckUpdatesImpl);
        StartDownload = ReactiveCommand.CreateFromTask(DownloadsImpl);
    }

    private async Task<Unit> DownloadsImpl() {
        return Unit.Default;
    }

    private async Task<Unit> CheckUpdatesImpl() {

        return Unit.Default;
    }

    private IObservable<Unit> StartServerImpl() {
        Launcher.Launch(path: @"binaries\xrEngine.exe",
            arguments: new List<string> {
                "-dedicated",
                "-i",
                @"-start server(belarus_lobby/fmp/timelimit=60) client(localhost)",
            });
        _isStartServer = true;
        return Observable.Return(Unit.Default);
    }

    private void PlayGameImpl() {
        if (_isStartServer) {
            Launcher.Launch(path: @"binaries\xrEngine.exe",
                arguments: new List<string> {
                @$"-start client(localhost/name={_userSettings.UserName})"
            });
            _windowManager.Close();
            return;
        }

        LauncherViewModel.StartGame();
    }
}
