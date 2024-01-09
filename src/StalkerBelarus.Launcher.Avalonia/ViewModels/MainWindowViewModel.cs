using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using StalkerBelarus.Launcher.Avalonia.Helpers;
using StalkerBelarus.Launcher.Avalonia.Manager;
using StalkerBelarus.Launcher.Core.Manager;
using StalkerBelarus.Launcher.Core.Models;
using StalkerBelarus.Launcher.Core.Storage;

namespace StalkerBelarus.Launcher.Avalonia.ViewModels;

public class MainWindowViewModel : ReactiveObject {
    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly ILocaleManager _localeManager;

    private readonly AuthorizationViewModel _authorizationViewModel;
    private readonly StartGameViewModel _startGameViewModel;
    private readonly UserManager _userManager;
    private readonly LauncherViewModel _launcherViewModel;
    
    [Reactive] public ReactiveObject PageViewModel { get; set; } = null!;

    public MainWindowViewModel(ILogger<MainWindowViewModel> logger, ILocaleManager localeManager,
        LauncherViewModel launcherViewModel,
        AuthorizationViewModel authorizationViewModel, StartGameViewModel startGameViewModel, UserManager userManager) {
        _logger = logger;
        _localeManager = localeManager;

        _authorizationViewModel = authorizationViewModel ?? throw new ArgumentNullException(nameof(authorizationViewModel));
        _startGameViewModel = startGameViewModel ?? throw new ArgumentNullException(nameof(startGameViewModel));

        _userManager = userManager;
        _launcherViewModel = launcherViewModel ?? throw new ArgumentNullException(nameof(launcherViewModel));

        if (_userManager is null) {
            throw new NullReferenceException("User manager object is null");
        }
        if (_userManager.UserSettings is null) {
            throw new NullReferenceException("User settings object is null");
        }

        if (File.Exists(FileLocations.UserSettingPath)
            && !string.IsNullOrEmpty(_userManager.UserSettings.Username)) {
            _localeManager.SetLocale(_userManager.UserSettings.Locale.Key);
            ShowLauncherImpl();
        } else {
            ShowAuthorizationImpl();
        }

        ProcessHelper.KillAllXrEngine();
    }

#if DEBUG
    public MainWindowViewModel() {
        _logger = null!;
        _localeManager = null!;
        _userManager = null!;
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
        // Update locale
        _startGameViewModel.SetupValidation();
        PageViewModel = _startGameViewModel;
    }
}
