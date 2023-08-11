using System.Reactive;
using System.Reactive.Linq;

using Avalonia;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using StalkerBelarus.Launcher.Core.Manager;
using StalkerBelarus.Launcher.Core.Models;

namespace StalkerBelarus.Launcher.Avalonia.ViewModels;

public class StartGameViewModel : ViewModelBase {
    private readonly ILogger<StartGameViewModel> _logger;
    private readonly UserSettings _userSettings;
    private readonly IWindowManager _windowManager;
    [Reactive] public string IpAddress { get; set; }

    public ReactiveCommand<Unit, Unit> StartGame { get; private set; } = null!;
    public ReactiveCommand<MainWindowViewModel, Unit> Back { get; private set; } = null!;
    
    public StartGameViewModel(ILogger<StartGameViewModel> logger, UserSettings userSettings, 
        IWindowManager windowManager) {
        _logger = logger;
        _userSettings = userSettings;
        IpAddress = _userSettings.IpAddress;
        _windowManager = windowManager;
        SetupCommands();
    }

#if DEBUG
    public StartGameViewModel() {
        _logger = null!;
        _userSettings = null!;
        _windowManager = null!;

        IpAddress = null!;
    }
#endif

    private void SetupCommands() {
        var canStartGame = this.WhenAnyValue(x => x.IpAddress,
                (ip) => !string.IsNullOrWhiteSpace(ip))
            .ObserveOn(RxApp.MainThreadScheduler)
            .DistinctUntilChanged();

        StartGame = ReactiveCommand.Create(StartGameImpl, canStartGame);
        Back = ReactiveCommand.Create<MainWindowViewModel>(BackImpl);
        
        StartGame.ThrownExceptions.Merge(Back.ThrownExceptions)
            .Throttle(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler)
            .Subscribe(OnCommandException);
    }
    
    private void StartGameImpl() {
        if (string.IsNullOrWhiteSpace(IpAddress)) {
            throw new Exception((string?) Application.Current?.Resources["LocalizedStrings.NoIpAddressEntered"]);
        }

        _userSettings.IpAddress = IpAddress;
        ConfigManager.SaveSettings(_userSettings);

        Core.Launcher.Launch(path: @"binaries\xrEngine.exe",
            arguments: new List<string> {
                @$"-start -center_screen -silent_error_mode client({_userSettings.IpAddress}/name={_userSettings.Username})"
            });
        _windowManager.Close();
    }

    private void BackImpl(MainWindowViewModel mainWindowViewModel) {
        mainWindowViewModel.ShowLauncherImpl();
    }
    
    private void OnCommandException(Exception exception)
        => _logger.LogError("{Message}", exception.Message);
}

