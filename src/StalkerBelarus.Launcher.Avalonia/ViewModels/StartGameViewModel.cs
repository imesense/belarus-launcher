using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Avalonia;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using StalkerBelarus.Launcher.Avalonia.Helpers;
using StalkerBelarus.Launcher.Avalonia.Manager;
using StalkerBelarus.Launcher.Core.Manager;
using StalkerBelarus.Launcher.Core.Models;

namespace StalkerBelarus.Launcher.Avalonia.ViewModels;

public class StartGameViewModel : ReactiveValidationObject, IDisposable {
    private readonly ILogger<StartGameViewModel> _logger;
    private readonly ILocaleManager _localeManager;
    private readonly UserSettings _userSettings;
    private readonly IWindowManager _windowManager;
    private readonly StartGameValidator _startGameValidator;
    private CompositeDisposable? _disposables = null;

    [Reactive] public string IpAddress { get; set; }

    public ReactiveCommand<Unit, Unit> StartGame { get; private set; } = null!;
    public ReactiveCommand<MainWindowViewModel, Unit> Back { get; private set; } = null!;
    
    public StartGameViewModel(ILogger<StartGameViewModel> logger, UserSettings userSettings, 
        IWindowManager windowManager, ILocaleManager localeManager, StartGameValidator startGameValidator) {
        _logger = logger;
        _userSettings = userSettings;
        IpAddress = _userSettings.IpAddress;
        _windowManager = windowManager;
        _startGameValidator = startGameValidator;
        _localeManager = localeManager;

        SetupCommands();
    }

#if DEBUG
    public StartGameViewModel() {
        _logger = null!;
        _userSettings = null!;
        _windowManager = null!;
        _localeManager = null!;
        _startGameValidator = null!;
        IpAddress = null!;
    }
#endif

    private void SetupCommands() {
        if (_disposables is null) {
            SetupValidation();
        }

        StartGame = ReactiveCommand.Create(StartGameImpl, this.IsValid());
        Back = ReactiveCommand.Create<MainWindowViewModel>(BackImpl);
        
        StartGame.ThrownExceptions.Merge(Back.ThrownExceptions)
            .Throttle(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler)
            .Subscribe(OnCommandException);
    }

    private void SetupValidation() {
        _disposables = new CompositeDisposable {
            _startGameValidator.EnsureIpAddressNotEmpty(this, _userSettings.Locale),
            _startGameValidator.EnsureValidIpAddressOrUrl(this, _userSettings.Locale)
        };
    }

    private void StartGameImpl() {
        if (string.IsNullOrWhiteSpace(IpAddress)) {
            throw new Exception(_localeManager.GetStringByKey("LocalizedStrings.NoIpAddressEntered",
                _userSettings.Locale));
        }

        _userSettings.IpAddress = IpAddress;
        ConfigManager.SaveSettings(_userSettings);

        var process = Core.Launcher.Launch(path: @"binaries\xrEngine.exe",
            arguments: new List<string> {
                @$"-start -center_screen -silent_error_mode client({_userSettings.IpAddress}/name={_userSettings.Username})"
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

