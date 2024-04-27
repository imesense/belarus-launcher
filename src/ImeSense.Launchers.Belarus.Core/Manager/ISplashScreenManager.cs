using ImeSense.Launchers.Belarus.Models;

namespace ImeSense.Launchers.Belarus.Core.Manager;

public interface ISplashScreenManager
{
    CancellationToken CancellationToken { get; }
    InformationMessage SplashScreenMessage { get; }
    int CurrentProgress { get; set; }
    int MaxProgress { get; set; }
    void UpdateInformation(InformationMessage splashScreenMessage);
    void Cancel();
}
