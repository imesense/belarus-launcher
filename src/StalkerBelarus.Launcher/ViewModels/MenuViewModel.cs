using StalkerBelarus.Launcher.Manager;

namespace StalkerBelarus.Launcher.ViewModels;

public class MenuViewModel : ReactiveObject
{
    private readonly IWindowManager _windowManager;

    public IReactiveCommand? Close { get; private set; }

    public MenuViewModel(IWindowManager windowManager)
    {
        _windowManager = windowManager;

        SetupCommands();
    }

    private void SetupCommands()
    {
        Close = ReactiveCommand.Create(() => _windowManager.Close());
    }
}
