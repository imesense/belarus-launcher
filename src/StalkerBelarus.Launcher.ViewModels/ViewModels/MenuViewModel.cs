using System.Diagnostics;

using StalkerBelarus.Launcher.ViewModels.Manager;

namespace StalkerBelarus.Launcher.ViewModels;

public class MenuViewModel : ReactiveObject {
    private readonly IWindowManager _windowManager;
    private readonly UserSettings _userSettings;

    public IReactiveCommand? Close { get; private set; }
    public IReactiveCommand? PlayGame { get; private set; }
    public IReactiveCommand? StartServer { get; private set; }

    public MenuViewModel(IWindowManager windowManager, UserSettings userSettings) {
        _windowManager = windowManager;
        _userSettings = userSettings;

        _userSettings = ConfigManager.LoadSettings();

        SetupCommands();
    }

    private void SetupCommands() {
        Close = ReactiveCommand.Create(_windowManager.Close);
        PlayGame = ReactiveCommand.Create(() => PlayGameImpl());
        StartServer = ReactiveCommand.Create(() => StartServerImpl());
    }

    private void StartServerImpl() =>
        Launcher.Launch(path: @"binaries\dedicated\xrEngine.exe", workingDirectory: @"binaries\",
            arguments: new List<string> {
                "-dedicated",
                "-i",
                @"-start server(belarus_lobby/fmp/timelimit=60) client(localhost)",
            });

    private void PlayGameImpl() =>
        Launcher.Launch(path: @"binaries\xrEngine.exe",
            arguments: new List<string> {
                @$"-start client({_userSettings.IpAdress}/name={_userSettings.UserName})"
            });
}
