using System.Reactive;

using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ImeSense.Launchers.Belarus.ViewModels;

public class SplashScreenViewModel : ReactiveObject
{
    private readonly CancellationTokenSource _cts = new();
    public CancellationToken CancellationToken => _cts.Token;

    private readonly IWindowManager _windowManager;
    public ReactiveCommand<Unit, Unit> Cancel { get; set; } = null!;
    [Reactive] public InformationMessage? InformationMessage { get; set; }
    [Reactive] public int Progress { get; set; } = 0;
    [Reactive] public int MaxProgress { get; set; } = 3;

    public SplashScreenViewModel()
    {
        _windowManager = null!;
    }

    public SplashScreenViewModel(IWindowManager windowManager) : base()
    {
        InformationMessage = new InformationMessage("Title", "Description");
        Cancel = ReactiveCommand.Create(CancelImpl);
        _windowManager = windowManager;
    }

    private void CancelImpl()
    {
        _cts.Cancel();
        _windowManager.Close();
    }
}
