using Belarus.Launcher.Core.Models;
using Belarus.Launcher.Core.Storage;

namespace Belarus.Launcher.ViewModels;

/// <summary>
/// Main view model
/// </summary>
public class MainViewModel : ViewModelBase, IScreen
{
    private readonly AuthorizationViewModel _authorizationViewModel;
    private readonly LauncherViewModel _launcherViewModel;

    public RoutingState Router { get; } = new();

    public MainViewModel(AuthorizationViewModel authorizationViewModel, LauncherViewModel launcherViewModel, UserSettings userSettings)
    {
        _authorizationViewModel = authorizationViewModel;
        _launcherViewModel = launcherViewModel;

        _authorizationViewModel.HostScreen = this;
        _launcherViewModel.HostScreen = this;

        if (!File.Exists(PathStorage.LauncherSetting) ||
            string.IsNullOrEmpty(userSettings.Username))
        {
            ShowAuthorization();
        }
        else
        {
            ShowLauncher();
        }
    }

    private void ShowLauncher()
    {
        Router.Navigate.Execute(_launcherViewModel).Subscribe();
    }

    private void ShowAuthorization()
    {
        Router.Navigate.Execute(_authorizationViewModel).Subscribe();
    }
}
