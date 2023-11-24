using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using DynamicData;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using StalkerBelarus.Launcher.Avalonia.Helpers;
using StalkerBelarus.Launcher.Avalonia.Manager;
using StalkerBelarus.Launcher.Core.Manager;
using StalkerBelarus.Launcher.Core.Models;
using StalkerBelarus.Launcher.Core.Storage;

namespace StalkerBelarus.Launcher.Avalonia.ViewModels;

public class AuthorizationViewModel : ReactiveValidationObject, IDisposable {
    private readonly ILogger<AuthorizationViewModel> _logger;
    private readonly ILocaleStorage _localeStorage;
    private readonly ILocaleManager _localeManager;

    private readonly IWindowManager _windowManager;
    private readonly UserSettings _userSettings;
    private readonly AuthenticationValidator _authenticationValidator;

    private CompositeDisposable? _disposables = null;

    [Reactive] public ObservableCollection<Locale> Languages { get; set; } = new();

    [Reactive] public Locale SelectedLanguage { get; set; } = new();

    [Reactive] public string Username { get; set; } = string.Empty;

    public ReactiveCommand<string, Unit> UpdateInterfaceCommand { get; private set; } = null!;
    public ReactiveCommand<MainWindowViewModel, Unit> ShowLauncher { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> Close { get; private set; } = null!;

    public AuthorizationViewModel(ILogger<AuthorizationViewModel> logger,
        ILocaleStorage localeStorage, ILocaleManager localeManager,
        IWindowManager windowManager, UserSettings userSettings,
        AuthenticationValidator authenticationValidator) {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _localeStorage = localeStorage;
        _localeManager = localeManager;
        _windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
        _userSettings = userSettings;
        _authenticationValidator = authenticationValidator;
        SetupBinding();
    }

#if DEBUG
    public AuthorizationViewModel() {
        _logger = null!;
        _localeStorage = null!;
        _localeManager = null!;
        _windowManager = null!;
        _userSettings = null!;
        _authenticationValidator = null!;
    }
#endif

    public void ShowLauncherImpl(MainWindowViewModel mainWindowViewModel) {
        var username = Username.Trim();
        if (string.IsNullOrWhiteSpace(username)) {
            throw new Exception(_localeManager.GetStringByKey("LocalizedStrings.UsernameNotEntered",
                SelectedLanguage.Key));
        }
        _userSettings.Username = username;
        _userSettings.Locale = SelectedLanguage.Key;
        ConfigManager.SaveSettings(_userSettings);

        mainWindowViewModel.ShowLauncherImpl();
    }

    private void SetupBinding() {
        Languages.AddRange(_localeStorage.GetLocales());
        if (_userSettings.Locale == string.Empty) {
            SelectedLanguage = Languages[0];
        }

        UpdateInterfaceCommand = ReactiveCommand.Create<string>(key => {
            _localeManager.SetLocale(key);
            _userSettings.Locale = SelectedLanguage.Key;
            _disposables?.Dispose();
            SetupValidation();
        });

        this.WhenAnyValue(x => x.SelectedLanguage.Key)
            .Where(key => !string.IsNullOrEmpty(key))
            .ObserveOn(RxApp.MainThreadScheduler)
            .InvokeCommand(UpdateInterfaceCommand);

        ShowLauncher = ReactiveCommand.Create<MainWindowViewModel>(ShowLauncherImpl, this.IsValid());
        Close = ReactiveCommand.Create(_windowManager.Close);

        ShowLauncher.ThrownExceptions.Merge(Close.ThrownExceptions)
            .Throttle(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler)
            .Subscribe(OnCommandException);
    }

    private void SetupValidation() {
        _disposables = new CompositeDisposable {
            _authenticationValidator.EnsureUsernameNotEmpty(this, SelectedLanguage.Key),
            _authenticationValidator.EnsureUsernameCorrectLength(this, SelectedLanguage.Key),
            _authenticationValidator.EnsureUsernameCorrectCharacters(this, SelectedLanguage.Key),
        };
    }

    private void OnCommandException(Exception exception)
        => _logger.LogError("{Message}", exception.Message);

    protected virtual void Dispose(bool disposing) {
        if (disposing) {
            if (_disposables is not null) {
                _disposables?.Dispose();
                _disposables = null;
            }
        }
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
