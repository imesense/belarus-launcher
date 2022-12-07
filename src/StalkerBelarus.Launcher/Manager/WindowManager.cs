using StalkerBelarus.Launcher.Helpers;
using StalkerBelarus.Launcher.ViewModels.Manager;

namespace StalkerBelarus.Launcher.Manager;

public class WindowManager : IWindowManager {
    public void Close() {
        ApplicationHelper.Close();
    }
}
