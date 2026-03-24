using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

using Belarus.Launcher.Core.Manager;

namespace Belarus.Launcher.Manager;

public class WindowManager : IWindowManager
{
    public void Close()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime app)
        {
            app.Shutdown();
        }
    }
}
