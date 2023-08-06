using System.Reactive;
using System.Reactive.Linq;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using StalkerBelarus.Launcher.Core.Manager;
using StalkerBelarus.Launcher.Core.Models;

namespace StalkerBelarus.Launcher.Avalonia.ViewModels;

public class AuthorizationViewModel : ViewModelBase {
    private readonly ILogger<AuthorizationViewModel> _logger;
    private readonly IWindowManager _windowManager;
    private readonly UserSettings _userSettings;
    
    [Reactive] public string Username { get; set; } = string.Empty;
    public ReactiveCommand<MainWindowViewModel, Unit> ShowLauncher { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> Close { get; private set; } = null!;

    public AuthorizationViewModel(ILogger<AuthorizationViewModel> logger, 
        IWindowManager windowManager, UserSettings userSettings) {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
        _userSettings = userSettings;

        SetupBinding();
    }

#if DEBUG
    public AuthorizationViewModel() {
        _logger = null!;
        _windowManager = null!;
        _userSettings = null!;
    }
#endif

    public void ShowLauncherImpl(MainWindowViewModel mainWindowViewModel) {
        if (string.IsNullOrWhiteSpace(Username)) {
            throw new Exception("Имя пользователя не введено!");
        }
        _userSettings.Username = Username;
        ConfigManager.SaveSettings(_userSettings);

        mainWindowViewModel.ShowLauncherImpl();
    }

    private void SetupBinding() {
        var canCreateUser = this.WhenAnyValue(x => x.Username,
                nickname => !string.IsNullOrWhiteSpace(nickname) && nickname.Length <= 22)
            .ObserveOn(RxApp.MainThreadScheduler)
            .DistinctUntilChanged();
        
        ShowLauncher = ReactiveCommand.Create<MainWindowViewModel>(ShowLauncherImpl, canCreateUser);
        Close = ReactiveCommand.Create(_windowManager.Close);
        
        ShowLauncher.ThrownExceptions.Merge(Close.ThrownExceptions)
            .Throttle(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler)
            .Subscribe(OnCommandException);
    }
    
    private void OnCommandException(Exception exception)
        => _logger.LogError("{Message}", exception.Message);
}
