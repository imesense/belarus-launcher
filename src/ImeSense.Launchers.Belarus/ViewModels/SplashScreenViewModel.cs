using System.Reactive;

using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.Helpers;
using ImeSense.Launchers.Belarus.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ImeSense.Launchers.Belarus.ViewModels;

public class SplashScreenViewModel : ReactiveObject
{
    private readonly IWindowManager _windowManager;
    ISplashScreenManager SplashScreen { get; set; }

    [Reactive] public InformationMessage InformationMessage { get; set; }
    [Reactive] public int Progress { get; set; }
    public int MaxProgress { get; private set; }

    public ReactiveCommand<Unit, Unit> Cancel { get; set; } = null!;
    
    public SplashScreenViewModel()
    {
        ExceptionHelper.ThrowIfEmptyConstructorNotInDesignTime($"{nameof(StartGameViewModel)}");

        _windowManager = null!;
        SplashScreen = null!;
    }

    public SplashScreenViewModel(IWindowManager windowManager, ISplashScreenManager splashScreen)
    {
        _windowManager = windowManager;
        SplashScreen = splashScreen;
        Progress = SplashScreen.CurrentProgress;
        MaxProgress = SplashScreen.MaxProgress;

        this.WhenAnyValue(
            x => x.SplashScreen.CurrentProgress,
            x => x.SplashScreen.SplashScreenMessage)
            .Subscribe(u => {
                Progress = u.Item1;
                InformationMessage = u.Item2;
            });

        Cancel = ReactiveCommand.Create(CancelImpl);
    }

    private void CancelImpl()
    {
        SplashScreen.Cancel();
        _windowManager.Close();
    }
}
