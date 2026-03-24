using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.ViewModels.Validators;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.SourceGenerators;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace ImeSense.Launchers.Belarus.ViewModels;

public partial class StartGameViewModel : ReactiveValidationObject, IDisposable
{
    private readonly ILogger<StartGameViewModel>? _logger;
    private readonly IWindowManager _windowManager;
    private readonly UserManager _userManager;
    private readonly StartGameViewModelValidator _startGameViewModelValidator;
    private CompositeDisposable? _disposables;

    public IApplicationLocaleManager Localization { get; private set; }
    [Reactive] public partial string IpAddress { get; set; }

    public ReactiveCommand<Unit, Unit> StartGame { get; private set; } = null!;
    public ReactiveCommand<MainWindowViewModel, Unit> Back { get; private set; } = null!;

    public StartGameViewModel(ILogger<StartGameViewModel>? logger, UserManager userManager,
        IWindowManager windowManager, IApplicationLocaleManager localization,
        StartGameViewModelValidator startGameViewModelValidator)
    {
        Localization = localization;
        _logger = logger;
        _logger?.LogInformation("StartGameViewModel ctor");

        _userManager = userManager;

        if (_userManager is null)
        {
            throw new NullReferenceException("User manager object is null");
        }
        if (_userManager.UserSettings is null)
        {
            throw new NullReferenceException("User settings object is null");
        }

        IpAddress = _userManager.UserSettings.IpAddress;

        _windowManager = windowManager;
        _startGameViewModelValidator = startGameViewModelValidator;

        SetupCommands();
    }

    private void SetupCommands()
    {
        StartGame = ReactiveCommand.Create(StartGameImpl, this.IsValid());
        Back = ReactiveCommand.Create<MainWindowViewModel>(BackImpl);

        this.WhenAnyValue(x => x.Localization.Locale)
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(x =>
            {
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
        if (_userManager is null)
        {
            throw new NullReferenceException("User manager object is null");
        }
        if (_userManager.UserSettings is null)
        {
            throw new NullReferenceException("User settings object is null");
        }

        if (string.IsNullOrWhiteSpace(IpAddress))
        {
            throw new Exception(Localization.GetStringByKey("LocalizedStrings.NoIpAddressEntered"));
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

    protected new virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            ValidationContext.Dispose();

            if (_disposables is not null)
            {
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
