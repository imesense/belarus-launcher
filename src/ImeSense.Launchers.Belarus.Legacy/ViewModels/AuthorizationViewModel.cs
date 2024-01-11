using System.Reactive.Linq;

using ReactiveUI.Fody.Helpers;

using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.Core.Models;
using ImeSense.Launchers.Belarus.Legacy.Manager;

namespace ImeSense.Launchers.Belarus.ViewModels;

public class AuthorizationViewModel : ViewModelBase, IRoutableViewModel {
    private readonly IWindowManager _windowManager;
    private readonly LauncherViewModel _launcherViewModel;
    private readonly UserSettings _userSettings;
    
    [Reactive] public string UserName { get; set; } = string.Empty;

    public ReactiveCommand<Unit, Unit> Next { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> Close { get; private set; } = null!;

    public string? UrlPathSegment { get; set; } = "";

    public IScreen HostScreen { get; set; } = null!;

    public AuthorizationViewModel(IWindowManager windowManager, LauncherViewModel launcherViewModel, UserSettings userSettings) {
        _windowManager = windowManager;

        if (launcherViewModel is null) {
            throw new ArgumentNullException(nameof(launcherViewModel));
        }

        _userSettings = userSettings;

        _launcherViewModel = launcherViewModel;
        _launcherViewModel.HostScreen = HostScreen;

        SetupBinding();
    }

    private void SetupBinding() {
        var canCreateUser = this.WhenAnyValue(x => x.UserName,
            (nickname) => !string.IsNullOrWhiteSpace(nickname) && nickname.Length <= 22)
            .ObserveOn(RxApp.MainThreadScheduler)
            .DistinctUntilChanged();

        Next = ReactiveCommand.Create(NextImpl, canCreateUser);
        Close = ReactiveCommand.Create(_windowManager.Close);
    }

    private void NextImpl() {
        if (HostScreen is null) {
            throw new ArgumentNullException(nameof(HostScreen));
        }
        if (string.IsNullOrWhiteSpace(UserName)) {
            throw new Exception("Имя пользователя не введено!");
        }

        _userSettings.Username = UserName;
        ConfigManager.SaveSettings(_userSettings);

        HostScreen.Router.Navigate.Execute(_launcherViewModel);
    }
}
