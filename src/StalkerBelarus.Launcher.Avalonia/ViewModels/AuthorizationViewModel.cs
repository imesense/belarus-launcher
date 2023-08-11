using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;

using Avalonia;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using StalkerBelarus.Launcher.Avalonia.Manager;
using StalkerBelarus.Launcher.Core.Manager;
using StalkerBelarus.Launcher.Core.Models;

namespace StalkerBelarus.Launcher.Avalonia.ViewModels;

public class AuthorizationViewModel : ViewModelBase {
    private readonly ILogger<AuthorizationViewModel> _logger;
    private readonly ILocaleManager _localeManager;

    private readonly IWindowManager _windowManager;
    private readonly UserSettings _userSettings;

    [Reactive]
    public ObservableCollection<Locale> Languages { get; set; } = new() {
        new Locale { Key = "ru", Title = "Русский", },
        new Locale { Key = "be", Title = "Беларуская", },
        new Locale { Key = "en", Title = "English", },
    };
    [Reactive]
    public string SelectedLanguageKey { get; set; } = string.Empty;
    [Reactive] public string Username { get; set; } = string.Empty;

    public ReactiveCommand<string, Unit> UpdateInterfaceCommand { get; private set; } = null!;
    public ReactiveCommand<MainWindowViewModel, Unit> ShowLauncher { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> Close { get; private set; } = null!;

    public AuthorizationViewModel(ILogger<AuthorizationViewModel> logger,
        ILocaleManager localeManager,
        IWindowManager windowManager, UserSettings userSettings) {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _localeManager = localeManager;
        _windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
        _userSettings = userSettings;

        SetupBinding();
    }

#if DEBUG
    public AuthorizationViewModel() {
        _logger = null!;
        _localeManager = null!;
        _windowManager = null!;
        _userSettings = null!;
    }
#endif

    public void ShowLauncherImpl(MainWindowViewModel mainWindowViewModel) {
        if (string.IsNullOrWhiteSpace(Username)) {
            throw new Exception((string?) Application.Current?.Resources["LocalizedStrings.UsernameNotEntered"]);
        }
        _userSettings.Username = Username;
        _userSettings.Locale = SelectedLanguageKey;
        ConfigManager.SaveSettings(_userSettings);

        mainWindowViewModel.ShowLauncherImpl();
    }

    private void SetupBinding() {
        UpdateInterfaceCommand = ReactiveCommand.Create<string>(key => {
            _localeManager.SetLocale(key);
        });

        this.WhenAnyValue(x => x.SelectedLanguageKey)
            .Where(key => !string.IsNullOrEmpty(key))
            .ObserveOn(RxApp.MainThreadScheduler)
            .InvokeCommand(UpdateInterfaceCommand);

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
