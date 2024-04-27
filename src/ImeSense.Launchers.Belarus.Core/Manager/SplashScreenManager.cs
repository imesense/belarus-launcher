using ImeSense.Launchers.Belarus.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ImeSense.Launchers.Belarus.Core.Manager;

public class SplashScreenManager : ReactiveObject, ISplashScreenManager
{
    private readonly CancellationTokenSource _cts = new();

    public CancellationToken CancellationToken { get; }
    [Reactive] public InformationMessage SplashScreenMessage { get; private set; }
    [Reactive] public int CurrentProgress { get; set; } = 0;
    public int MaxProgress { get; set; }

    public SplashScreenManager()
    {
        CancellationToken = _cts.Token;
        SplashScreenMessage = new InformationMessage("Title", "Description");
    }

    public void Cancel()
    {
        _cts.Cancel();
    }

    public void UpdateInformation(InformationMessage splashScreenMessage)
    {
        CurrentProgress++;
        SplashScreenMessage = splashScreenMessage;
    }
}
