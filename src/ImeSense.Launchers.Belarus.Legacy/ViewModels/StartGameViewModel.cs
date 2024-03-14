using System.Reactive.Linq;

using ReactiveUI.Fody.Helpers;

using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.Core.Models;
using ImeSense.Launchers.Belarus.Legacy.Manager;

namespace ImeSense.Launchers.Belarus.ViewModels;

public class StartGameViewModel : ViewModelBase, IRoutableViewModel {
    private readonly IWindowManager _windowManager;
    private readonly UserSettings _userSettings;

    [Reactive] public string IpAddress { get; set; } = string.Empty;

    public ReactiveCommand<Unit, Unit> StartGame { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> Back { get; private set; } = null!;

    public string? UrlPathSegment { get; set; } = "StartGameViewModel";

    public IScreen HostScreen { get; set; } = null!;

    public StartGameViewModel(IWindowManager windowManager, UserSettings userSettings) {
        _windowManager = windowManager;
        _userSettings = userSettings;

        IpAddress = _userSettings.IpAddress ?? "";

        SetupBinding();
    }

    private void SetupBinding() {
        var canStartGame = this.WhenAnyValue(x => x.IpAddress,
            (ip) => !string.IsNullOrWhiteSpace(ip))
            .ObserveOn(RxApp.MainThreadScheduler)
            .DistinctUntilChanged();

        StartGame = ReactiveCommand.Create(StartGameImpl, canStartGame);
        Back = ReactiveCommand.Create(BackImpl);
    }

    private void StartGameImpl() {
        if (_userSettings is null) {
            throw new NullReferenceException("User manager object is null");
        }

        if (string.IsNullOrWhiteSpace(IpAddress)) {
            throw new Exception("Ip-адрес не введен!");
        }

        _userSettings.IpAddress = IpAddress;
        ConfigManager.SaveSettings(_userSettings);

        var gameProcess = Core.Launcher.Launch(path: @"binaries\xrEngine.exe", arguments: [
            @$"-start -center_screen -silent_error_mode client({_userSettings.IpAddress}/name={_userSettings.Username})"
        ]);
        gameProcess?.Start();
        BackImpl();

        return;
    }

    private void BackImpl() {
        _windowManager.Close();
    }
}
