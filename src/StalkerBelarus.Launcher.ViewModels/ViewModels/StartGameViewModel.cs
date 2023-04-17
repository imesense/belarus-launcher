using System.Net;
using System.Reactive.Linq;
using ReactiveUI.Fody.Helpers;

using StalkerBelarus.Launcher.ViewModels.Manager;

namespace StalkerBelarus.Launcher.ViewModels; 

public class StartGameViewModel : ReactiveObject, IRoutableViewModel {
    private readonly IWindowManager _windowManager;

    private UserSettings UserSettings { get; set; }
    [Reactive] public string IpAddress { get; set; } = string.Empty;

    public ReactiveCommand<Unit, Unit> StartGame { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> Back { get; private set; } = null!;

    public string? UrlPathSegment { get; set; } = "StartGameViewModel";

    public IScreen HostScreen { get; set; } = null!;

    public StartGameViewModel(IWindowManager windowManager, UserSettings userSettings) {
        _windowManager = windowManager;
        UserSettings = userSettings;
        IpAddress = UserSettings.IpAdress;

        SetupBinding();
    }

    private void SetupBinding() {
        var canStartGame = this.WhenAnyValue(x => x.IpAddress,
            (ip) => !string.IsNullOrWhiteSpace(ip))
            .ObserveOn(RxApp.MainThreadScheduler)
            .DistinctUntilChanged();

        StartGame = ReactiveCommand.Create(() => StartGameImpl(), canStartGame);
        Back = ReactiveCommand.Create(() => BackImpl());
    }

    private void StartGameImpl() {
        if (string.IsNullOrWhiteSpace(IpAddress)) {
            throw new Exception("Ip-адрес не введен!");
        }

        UserSettings.IpAdress = IpAddress;
        ConfigManager.SaveSettings(UserSettings);

        Launcher.Launch(path: @"binaries\xrEngine.exe",
        arguments: new List<string> {
            @$"-start client({UserSettings.IpAdress}/name={UserSettings.UserName})"
        });
        BackImpl();

        return;
    }

    private void BackImpl() {
        _windowManager.Close();
    }
}
