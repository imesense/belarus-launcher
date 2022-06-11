using StalkerBelarus.Launcher.Helpers;
using System.Windows.Input;

namespace StalkerBelarus.Launcher.ViewModels;

public class MenuViewModel : ReactiveObject
{
    public IReactiveCommand? Close { get; private set; }

    public MenuViewModel()
    {
        SetupCommands();
    }

    private void SetupCommands()
    {
        Close = ReactiveCommand.Create(() => ApplicationHelper.Close());
    }
}
