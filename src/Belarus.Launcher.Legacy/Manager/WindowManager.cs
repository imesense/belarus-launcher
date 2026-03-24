using Belarus.Launcher.Core.Manager;
using Belarus.Launcher.Helpers;

namespace Belarus.Launcher.Manager;

public class WindowManager : IWindowManager
{
    public void Close()
    {
        ApplicationHelper.Close();
    }
}
