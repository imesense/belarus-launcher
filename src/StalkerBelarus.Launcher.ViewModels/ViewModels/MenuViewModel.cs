using System.Reactive.Linq;

using ReactiveUI.Fody.Helpers;

using StalkerBelarus.Launcher.Core.Models;
using StalkerBelarus.Launcher.ViewModels.Manager;

namespace StalkerBelarus.Launcher.ViewModels;

public class MenuViewModel : ReactiveObject {
    private readonly IWindowManager _windowManager;
    private readonly UserSettings _userSettings;
    private readonly DownloadManager _downloadService;
    public LauncherViewModel LauncherViewModel { get; set; }
    private bool _isStartServer = false;

    [Reactive] public bool IsVisibleDownload { get; set; } = true;
    [Reactive] public bool IsVisiblePlayGame { get; set; } = false;
    [Reactive] public bool IsDownloadStart { get; set; } = false;
    [Reactive] public bool IsDownloadCheak { get; set; } = false;

    public ReactiveCommand<Unit, Unit> Close { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> PlayGame { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> StartServer { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> CheckUpdates { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> StartDownload { get; private set; } = null!;

    public MenuViewModel(IWindowManager windowManager, UserSettings userSettings, DownloadManager downloadService) {
        _downloadService = downloadService;
        _windowManager = windowManager;
        _userSettings = userSettings;

        SetupCommands();
    }

    private void SetupCommands() {
        var result = _downloadService.CheckFiles();
        if(result) {
            IsVisiblePlayGame = File.Exists(Directory.GetCurrentDirectory() + "binaries\\xrengine.exe");
            IsVisibleDownload = !IsVisiblePlayGame;
        } else {
            IsVisibleDownload = false;
            IsVisiblePlayGame = true;
        }
        Close = ReactiveCommand.Create(_windowManager.Close);
        PlayGame = ReactiveCommand.Create(() => PlayGameImpl());
        StartServer = ReactiveCommand.CreateFromObservable(StartServerImpl);
        CheckUpdates = ReactiveCommand.CreateFromTask(DownloadsImpl);
        StartDownload = ReactiveCommand.CreateFromTask(DownloadsImpl);
    }

    private async Task<Unit> DownloadsImpl() {
            IsVisibleDownload = false;
            IsVisiblePlayGame = false;
            IsDownloadStart = false;
            IsDownloadCheak = true;
            await Task.Run(
            () => {
                if (_downloadService.CheckFiles()) {
                    IsDownloadStart = true;
                    IsDownloadCheak = false;
                    _downloadService.CheckFiles(true);
                }
                IsDownloadStart = false;
                IsVisiblePlayGame = true;
                IsVisibleDownload = false;
                IsDownloadCheak = false;
            });
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
