using System.Reactive;

using ImeSense.Launchers.Belarus.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ImeSense.Launchers.Belarus.ViewModels;

public class SplashScreenViewModel : ReactiveObject
{
    private readonly CancellationTokenSource _cts = new();

    public CancellationToken CancellationToken => _cts.Token;
    public ReactiveCommand<Unit, Unit> Cancel { get; set; } = null!;
    [Reactive] public InformationMessage? InformationMessage { get; set; }
    [Reactive] public int Progress { get; set; } = 0;
    [Reactive] public int MaxProgress { get; set; } = 3;

    public SplashScreenViewModel()
    {
        InformationMessage = new InformationMessage("Title", "Description");
        Cancel = ReactiveCommand.Create(CancelImpl);
    }

    private void CancelImpl()
    {
        _cts.Cancel();
    }
}
