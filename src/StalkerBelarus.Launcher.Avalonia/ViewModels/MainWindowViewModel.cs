using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using StalkerBelarus.Launcher.Avalonia.Helpers;
using StalkerBelarus.Launcher.Avalonia.Manager;
using StalkerBelarus.Launcher.Core;
using StalkerBelarus.Launcher.Core.Models;

namespace StalkerBelarus.Launcher.Avalonia.ViewModels;

public class MainWindowViewModel : ViewModelBase {
    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly ILocaleManager _localeManager;

    private readonly AuthorizationViewModel _authorizationViewModel;
    private readonly StartGameViewModel _startGameViewModel;
    private readonly LauncherViewModel _launcherViewModel;
    
    [Reactive] public ReactiveObject PageViewModel { get; set; } = null!;

    public MainWindowViewModel(ILogger<MainWindowViewModel> logger, ILocaleManager localeManager,
        LauncherViewModel launcherViewModel,
        AuthorizationViewModel authorizationViewModel, StartGameViewModel startGameViewModel, UserSettings userSettings) {
        _logger = logger;
        _localeManager = localeManager;

        _authorizationViewModel = authorizationViewModel ?? throw new ArgumentNullException(nameof(authorizationViewModel));
        _startGameViewModel = startGameViewModel ?? throw new ArgumentNullException(nameof(startGameViewModel));
        _launcherViewModel = launcherViewModel ?? throw new ArgumentNullException(nameof(launcherViewModel));

        if (File.Exists(FileLocations.UserSettingPath) 
            && !string.IsNullOrEmpty(userSettings.Username)) {
            ShowLauncherImpl();

            _localeManager.SetLocale(userSettings.Locale);
        } else {
            ShowAuthorizationImpl();
        }
        
        ProcessHelper.KillAllXrEngine();
    }

#if DEBUG
    public MainWindowViewModel() {
        _logger = null!;
        _localeManager = null!;

        _authorizationViewModel = null!;
        _startGameViewModel = null!;
        _launcherViewModel = null!;
    }
#endif

    public void ShowLauncherImpl() {
        _launcherViewModel.SelectMenu();
        PageViewModel = _launcherViewModel;
    }

    public void ShowAuthorizationImpl() {
        PageViewModel = _authorizationViewModel;
    }
    
    public void ShowStartGameImpl() {
        PageViewModel = _startGameViewModel;
    }
}
