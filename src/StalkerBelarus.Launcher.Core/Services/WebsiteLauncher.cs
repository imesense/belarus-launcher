using System.Diagnostics;

namespace StalkerBelarus.Launcher.Core.Services;

public class WebsiteLauncher : IWebsiteLauncher {
    public void OpenWebsite(string url) {
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }
}
