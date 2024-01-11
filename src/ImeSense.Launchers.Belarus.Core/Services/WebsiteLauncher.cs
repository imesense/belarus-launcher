using System.Diagnostics;

namespace ImeSense.Launchers.Belarus.Core.Services;

public class WebsiteLauncher : IWebsiteLauncher {
    public void OpenWebsite(string url) {
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }
}
