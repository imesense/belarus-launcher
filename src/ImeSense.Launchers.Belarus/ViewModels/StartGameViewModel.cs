using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.ViewModels.Validators;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace ImeSense.Launchers.Belarus.ViewModels;

public class StartGameViewModel : ReactiveValidationObject, IDisposable
{
    private readonly ILogger<StartGameViewModel>? _logger;
    private readonly IApplicationLocaleManager _localeManager;
    private readonly IWindowManager _windowManager;
    private readonly UserManager _userManager;
    private readonly StartGameViewModelValidator _startGameViewModelValidator;
    private CompositeDisposable? _disposables;

    [Reactive] public string IpAddress { get; set; }

    public ReactiveCommand<Unit, Unit> StartGame { get; private set; } = null!;
    public ReactiveCommand<MainWindowViewModel, Unit> Back { get; private set; } = null!;

    public StartGameViewModel(ILogger<StartGameViewModel>? logger, UserManager userManager,
        IWindowManager windowManager, IApplicationLocaleManager localeManager,
        StartGameViewModelValidator startGameViewModelValidator)
    {
        _logger = logger;
        _logger?.LogInformation("StartGameViewModel ctor");

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
    
    private void SetupCommands()
    {
        StartGame = ReactiveCommand.Create(StartGameImpl, this.IsValid());
        Back = ReactiveCommand.Create<MainWindowViewModel>(BackImpl);

        StartGame.ThrownExceptions.Merge(Back.ThrownExceptions)
            .Throttle(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler)
            .Subscribe(OnCommandException);

        this.WhenAnyValue(x => x._localeManager.Locale)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(x => {
                _disposables?.Dispose();
                SetupValidation();
            });
    }

    private void SetupValidation()
    {
        _logger?.LogInformation("StartGameViewModel: setup validation");

        _disposables = [
            _startGameViewModelValidator.EnsureIpAddressNotEmpty(this),
            _startGameViewModelValidator.EnsureValidIpAddressOrUrl(this)
        ];
    }

    private void StartGameImpl()
    {
        if (_userManager is null) {
            throw new NullReferenceException("User manager object is null");
        }
        if (_userManager.UserSettings is null) {
            throw new NullReferenceException("User settings object is null");
        }

        if (string.IsNullOrWhiteSpace(IpAddress)) {
            throw new Exception(_localeManager.GetStringByKey("LocalizedStrings.NoIpAddressEntered"));
        }

        _userManager.UserSettings.IpAddress = IpAddress;
        _userManager.Save();

        var process = Core.Launcher.Launch(path: @"binaries\xrEngine.exe",
            arguments: [
                @$"-start -center_screen -silent_error_mode client({_userManager.UserSettings.IpAddress}/name={ _userManager.UserSettings.Username})"
            ]);

        process?.Start();
        _windowManager.Close();
    }

    private void BackImpl(MainWindowViewModel mainWindowViewModel)
    {
        mainWindowViewModel.ShowLauncherImpl();
    }

    private void OnCommandException(Exception exception)
        => _logger?.LogError("{Message}", exception.Message);

    protected new virtual void Dispose(bool disposing)
    {
        if (disposing) {
            ValidationContext.Dispose();

            if (_disposables is not null) {
                _disposables?.Dispose();
                _disposables = null;
            }
        }
    }

    public new void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
