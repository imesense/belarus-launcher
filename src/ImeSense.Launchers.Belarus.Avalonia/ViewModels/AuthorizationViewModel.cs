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

using ImeSense.Launchers.Belarus.Avalonia.ViewModels.Validators;
using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.Core.Models;
using ImeSense.Launchers.Belarus.Core.Storage;

namespace ImeSense.Launchers.Belarus.Avalonia.ViewModels;

public class AuthorizationViewModel : ReactiveValidationObject, IDisposable {
    private readonly ILogger<AuthorizationViewModel> _logger;
    private readonly ILauncherStorage _launcherStorage;
    private readonly ILocaleManager _localeManager;

    private readonly IWindowManager _windowManager;
    private readonly UserManager _userManager;
    private readonly AuthenticationViewModelValidator _authenticationViewModelValidator;
    private readonly LauncherViewModel _launcherViewModel;
    private CompositeDisposable? _disposables = null;

    [Reactive] public ObservableCollection<Locale> Languages { get; set; } = new();

    [Reactive] public Locale SelectedLanguage { get; set; } = new();

    [Reactive] public string Username { get; set; } = string.Empty;

    public ReactiveCommand<string, Unit> UpdateInterfaceCommand { get; private set; } = null!;
    public ReactiveCommand<MainWindowViewModel, Unit> ShowLauncher { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> Close { get; private set; } = null!;

    public AuthorizationViewModel(ILogger<AuthorizationViewModel> logger,
        ILauncherStorage launcherStorage, ILocaleManager localeManager,
        IWindowManager windowManager, UserManager userManager,
        AuthenticationViewModelValidator authenticationViewModelValidator,
        LauncherViewModel launcherViewModel) {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _launcherStorage = launcherStorage;
        _localeManager = localeManager;
        _windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
        _userManager = userManager;
        _authenticationViewModelValidator = authenticationViewModelValidator;
        _launcherViewModel = launcherViewModel;
    }

#if DEBUG
    public AuthorizationViewModel() {
        _logger = null!;
        _launcherStorage = null!;
        _localeManager = null!;
        _windowManager = null!;
        _userManager = null!;
        _authenticationViewModelValidator = null!;
        _launcherViewModel = null!;
    }
#endif

    public void ShowLauncherImpl(MainWindowViewModel mainWindowViewModel) {
        var username = Username.Trim();
        if (string.IsNullOrWhiteSpace(username)) {
            throw new Exception(_localeManager.GetStringByKey("LocalizedStrings.UsernameNotEntered",
                SelectedLanguage.Key));
        }

        if (_userManager is null) {
            throw new NullReferenceException("User manager object is null");
        }
        if (_userManager.UserSettings is null) {
            throw new NullReferenceException("User settings object is null");
        }
        if (_userManager.UserSettings.Locale is null) {
            throw new NullReferenceException("User settings locale object is null");
        }

        _userManager.UserSettings.Username = username;
        _userManager.UserSettings.Locale = SelectedLanguage;
        _userManager.Save();

        if (!File.Exists(FileLocations.GameUser)) {
            using var writer = new StreamWriter(FileLocations.GameUser, true);
            writer.WriteLine($"language {SelectedLanguage.Key}");
        }

        mainWindowViewModel.ShowLauncherImpl();
    }

    public void UpdateNews() {
        _logger.LogInformation("Update News");

        if (_userManager is null) {
            throw new NullReferenceException("User manager object is null");
        }
        if (_userManager.UserSettings is null) {
            throw new NullReferenceException("User settings object is null");
        }
        if (_userManager.UserSettings.Locale is null) {
            throw new NullReferenceException("User settings locale object is null");
        }
        var news = _launcherStorage?.NewsContents?.FirstOrDefault(x => x.Locale!.Key.Equals(_userManager.UserSettings.Locale.Key));
        if (news is not null) {
            if (news.NewsContents is not null) {
                _launcherViewModel.NewsSliderViewModel.SetNews(news.NewsContents);
            }
        }
    }

    public void SetupBinding() {
        if (_userManager is null) {
            throw new NullReferenceException("User manager object is null");
        }
        if (_userManager.UserSettings is null) {
            throw new NullReferenceException("User settings object is null");
        }
        if (_userManager.UserSettings.Locale is null) {
            throw new NullReferenceException("User settings locale object is null");
        }

        Languages.AddRange(_launcherStorage.Locales);
        if (_userManager.UserSettings.Locale.Key == string.Empty) {
            SelectedLanguage = Languages[0];
        } else {
            SelectedLanguage = Languages.FirstOrDefault(x => x.Key.Equals(_userManager.UserSettings.Locale.Key)) ?? Languages[0];
        }

        UpdateInterfaceCommand = ReactiveCommand.Create<string>(key => {
            _localeManager.SetLocale(key);
            _userManager.UserSettings.Locale = SelectedLanguage ?? Languages[0];
            UpdateNews();
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
            _authenticationViewModelValidator.EnsureUsernameNotEmpty(this),
            _authenticationViewModelValidator.EnsureUsernameCorrectLength(this),
            _authenticationViewModelValidator.EnsureUsernameCorrectCharacters(this),
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
