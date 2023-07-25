using Microsoft.Extensions.Logging;

namespace StalkerBelarus.Launcher.Avalonia.ViewModels;

public class LauncherViewModel : ViewModelBase {
    private readonly ILogger<LauncherViewModel> _logger;

    public LauncherViewModel(ILogger<LauncherViewModel> logger) {
        _logger = logger;
    }
}
