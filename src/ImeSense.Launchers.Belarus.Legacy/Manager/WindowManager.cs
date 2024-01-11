using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.Helpers;

namespace ImeSense.Launchers.Belarus.Manager;

public class WindowManager : IWindowManager {
    public void Close() {
        ApplicationHelper.Close();
    }
}
