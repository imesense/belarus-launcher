using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

using ImeSense.Launchers.Belarus.Core.Manager;

namespace ImeSense.Launchers.Belarus.Avalonia.Manager; 

public class WindowManager : IWindowManager {
    public void Close() {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime app) {
            app.Shutdown();
        }
    }
}
