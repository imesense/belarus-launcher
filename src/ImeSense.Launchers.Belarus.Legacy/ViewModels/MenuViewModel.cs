using ReactiveUI.Fody.Helpers;

using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.Core.Models;

namespace ImeSense.Launchers.Belarus.ViewModels;

public class MenuViewModel : ViewModelBase {
    private readonly IWindowManager _windowManager;
    private readonly UserManager _userManager;
    private readonly DownloadManager _downloadService;

    public LauncherViewModel? LauncherViewModel { get; set; }

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

    public MenuViewModel(IWindowManager windowManager, UserManager userManager, DownloadManager downloadService) {
        _downloadService = downloadService;
        _windowManager = windowManager;
        _userManager = userManager;

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
        PlayGame = ReactiveCommand.Create(PlayGameImpl);
        StartServer = ReactiveCommand.Create(StartServerImpl);
        CheckUpdates = ReactiveCommand.CreateFromTask(DownloadsImpl);
        StartDownload = ReactiveCommand.CreateFromTask(DownloadsImpl);
    }

    private async Task DownloadsImpl() {
        IsVisibleDownload = false;
        IsVisiblePlayGame = false;
        IsDownloadStart = false;
        IsDownloadCheak = true;

        await Task.Run(() => {
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

    private void PlayGameImpl() {
        if (_userManager is null) {
            throw new NullReferenceException("User manager object is null");
        }
        if (_userManager.UserSettings is null) {
            throw new NullReferenceException("User settings object is null");
        }

        if (_isStartServer) {
            Core.Launcher.Launch(path: @"binaries\xrEngine.exe",
                arguments: new List<string> {
                @$"-start client(localhost/name={_userManager.UserSettings.Username})"
            });
            _windowManager.Close();
            return;
        }

        LauncherViewModel?.StartGame();
    }
}
