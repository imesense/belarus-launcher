using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

using StalkerBelarus.Launcher.Core.Manager;

namespace StalkerBelarus.Launcher.Avalonia.Manager; 

public class WindowManager : IWindowManager {
    public void Close() {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime app) {
            app.Shutdown();
        }
    }
}
