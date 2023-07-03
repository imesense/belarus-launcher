using StalkerBelarus.Launcher.Core.Manager;
using StalkerBelarus.Launcher.Helpers;

namespace StalkerBelarus.Launcher.Manager;

public class WindowManager : IWindowManager {
    public void Close() {
        ApplicationHelper.Close();
    }
}
