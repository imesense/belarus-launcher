using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using ImeSense.Launchers.Belarus.Avalonia.Helpers;
using ImeSense.Launchers.Belarus.Avalonia.ViewModels.Validators;
using ImeSense.Launchers.Belarus.Core.Manager;

namespace ImeSense.Launchers.Belarus.Avalonia.ViewModels;

public class StartGameViewModel : ReactiveValidationObject, IDisposable {
    private readonly ILogger<StartGameViewModel> _logger;
    private readonly UserManager _userManager;
    private readonly ILocaleManager _localeManager;
    private readonly IWindowManager _windowManager;
    private readonly StartGameViewModelValidator _startGameViewModelValidator;

    private CompositeDisposable? _disposables = null;

    [Reactive] public string IpAddress { get; set; }

    public ReactiveCommand<Unit, Unit> StartGame { get; private set; } = null!;
    public ReactiveCommand<MainWindowViewModel, Unit> Back { get; private set; } = null!;
    
    public StartGameViewModel(ILogger<StartGameViewModel> logger, UserManager userManager,
        IWindowManager windowManager, ILocaleManager localeManager,
        StartGameViewModelValidator startGameViewModelValidator) {
        _logger = logger;
        _logger.LogInformation("StartGameViewModel ctor");

        _userManager = userManager;

        if (_userManager is null) {
            throw new NullReferenceException("User manager object is null");
        }
        if (_userManager.UserSettings is null) {
            throw new NullReferenceException("User settings object is null");
        }

        IpAddress = _userManager.UserSettings.IpAddress;

        _windowManager = windowManager;
        _startGameViewModelValidator = startGameViewModelValidator;
        _localeManager = localeManager;

        SetupCommands();
    }

    public StartGameViewModel() {
        ExceptionHelper.ThrowIfEmptyConstructorNotInDesignTime($"{nameof(StartGameViewModel)}");

        _logger = null!;
        _userManager = null!;
        _windowManager = null!;
        _localeManager = null!;
        _startGameViewModelValidator = null!;

        IpAddress = null!;
    }

    private void SetupCommands() {
        StartGame = ReactiveCommand.Create(StartGameImpl, this.IsValid());
        Back = ReactiveCommand.Create<MainWindowViewModel>(BackImpl);

        StartGame.ThrownExceptions.Merge(Back.ThrownExceptions)
            .Throttle(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler)
            .Subscribe(OnCommandException);
    }

    public void SetupValidation() {
        _logger.LogInformation("StartGameViewModel SetupValidation");
        _disposables?.Dispose();
        _disposables = new CompositeDisposable {
            _startGameViewModelValidator.EnsureIpAddressNotEmpty(this),
            _startGameViewModelValidator.EnsureValidIpAddressOrUrl(this)
        };
    }

    private void StartGameImpl() {
        if (_userManager is null) {
            throw new NullReferenceException("User manager object is null");
        }
        if (_userManager.UserSettings is null) {
            throw new NullReferenceException("User settings object is null");
        }
        if (_userManager.UserSettings.Locale is null) {
            throw new NullReferenceException("User settings locale object is null");
        }

        if (string.IsNullOrWhiteSpace(IpAddress)) {
            throw new Exception(_localeManager.GetStringByKey("LocalizedStrings.NoIpAddressEntered",
                _userManager.UserSettings.Locale.Key));
        }

        _userManager.UserSettings.IpAddress = IpAddress;
        _userManager.Save();

        var process = Core.Launcher.Launch(path: @"binaries\xrEngine.exe",
            arguments: new List<string> {
                @$"-start -center_screen -silent_error_mode client({_userManager.UserSettings.IpAddress}/name={ _userManager.UserSettings.Username})"
            });

        process?.Start();
        _windowManager.Close();
    }

    private void BackImpl(MainWindowViewModel mainWindowViewModel) {
        mainWindowViewModel.ShowLauncherImpl();
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

