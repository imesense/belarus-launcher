using System.Reactive.Linq;

using ReactiveUI.Fody.Helpers;

using StalkerBelarus.Launcher.Core.Manager;

namespace StalkerBelarus.Launcher.ViewModels;

public class StartGameViewModel : ViewModelBase, IRoutableViewModel {
    private readonly IWindowManager _windowManager;
    private readonly UserManager _userManager;

    [Reactive] public string IpAddress { get; set; } = string.Empty;

    public ReactiveCommand<Unit, Unit> StartGame { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> Back { get; private set; } = null!;

    public string? UrlPathSegment { get; set; } = "StartGameViewModel";

    public IScreen HostScreen { get; set; } = null!;

    public StartGameViewModel(IWindowManager windowManager, UserManager userManager) {
        _windowManager = windowManager;
        _userManager = userManager;

        IpAddress = _userManager.UserSettings?.IpAddress ?? "";

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
        if (_userManager is null) {
            throw new NullReferenceException("User manager object is null");
        }
        if (_userManager.UserSettings is null) {
            throw new NullReferenceException("User settings object is null");
        }

        if (string.IsNullOrWhiteSpace(IpAddress)) {
            throw new Exception("Ip-адрес не введен!");
        }

        _userManager.UserSettings.IpAddress = IpAddress;
        _userManager.Save();

        Core.Launcher.Launch(path: @"binaries\xrEngine.exe",
        arguments: new List<string> {
            @$"-start -center_screen -silent_error_mode client({_userManager.UserSettings.IpAddress}/name={_userManager.UserSettings.Username})"
        });
        BackImpl();

        return;
    }

    private void BackImpl() {
        _windowManager.Close();
    }
}
