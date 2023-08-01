using Microsoft.Extensions.Logging;

using ReactiveUI.Fody.Helpers;

using StalkerBelarus.Launcher.Core;
using StalkerBelarus.Launcher.Core.Models;

namespace StalkerBelarus.Launcher.Avalonia.ViewModels;

public class MainWindowViewModel : ViewModelBase {
    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly AuthorizationViewModel _authorizationViewModel;
    private readonly LauncherViewModel _launcherViewModel;
    
    [Reactive] public ViewModelBase PageViewModel { get; set; } = null!;

    public MainWindowViewModel(ILogger<MainWindowViewModel> logger, LauncherViewModel launcherViewModel, 
        AuthorizationViewModel authorizationViewModel, UserSettings userSettings) {
        _logger = logger;
        _authorizationViewModel = authorizationViewModel ?? throw new ArgumentNullException(nameof(authorizationViewModel));
        _launcherViewModel = launcherViewModel ?? throw new ArgumentNullException(nameof(launcherViewModel));

        if (File.Exists(FileLocations.UserSettingPath) 
            && !string.IsNullOrEmpty(userSettings.Username)) {
            ShowLauncherImpl();
        } else {
            ShowAuthorizationImpl();
        }
    }

    public void ShowLauncherImpl() {
        _launcherViewModel.SelectMenu();
        PageViewModel = _launcherViewModel;
    }

    public void ShowAuthorizationImpl() {
        PageViewModel = _authorizationViewModel;
    }
}
