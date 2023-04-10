using StalkerBelarus.Launcher.ViewModels.Manager;

namespace StalkerBelarus.Launcher.ViewModels;

/// <summary>
/// Main view model
/// </summary>
public class MainViewModel : ReactiveObject, IScreen {
    private readonly AuthorizationViewModel _authorizationViewModel;
    private readonly LauncherViewModel _launcherViewModel;
    private readonly UserSettings _userSettings;

    public RoutingState Router { get; } = new();

    public MainViewModel(AuthorizationViewModel authorizationViewModel,
        LauncherViewModel launcherViewModel, UserSettings userSettings) {
        _authorizationViewModel = authorizationViewModel ?? throw new ArgumentNullException(nameof(authorizationViewModel));
        _launcherViewModel = launcherViewModel ?? throw new ArgumentNullException(nameof(launcherViewModel)); ;

        _authorizationViewModel.HostScreen = this;
        _launcherViewModel.HostScreen = this;

        _userSettings = userSettings;

        if (!File.Exists(ConfigManager.Path)) {
            ShowAuthorization();
        }

        ShowLauncher();

        _userSettings = ConfigManager.LoadSettings();
    }

    private void ShowLauncher() {
        Router.Navigate.Execute(_launcherViewModel).Subscribe();
    }

    private void ShowAuthorization() {
        Router.Navigate.Execute(_authorizationViewModel).Subscribe();
    }
}
