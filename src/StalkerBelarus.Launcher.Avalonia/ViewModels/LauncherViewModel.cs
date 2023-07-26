using Microsoft.Extensions.Logging;

using StalkerBelarus.Launcher.Avalonia.Helpers;

namespace StalkerBelarus.Launcher.Avalonia.ViewModels;

public class LauncherViewModel : ViewModelBase {
    private readonly ILogger<LauncherViewModel> _logger;
    
    public string AppVersion { get; set; }
    public string CompanyName { get; set; }

    public LauncherViewModel(ILogger<LauncherViewModel> logger) {
        _logger = logger;
        
        AppVersion = ApplicationHelper.GetAppVersion();
        CompanyName = (char)0169 + ApplicationHelper.GetCompanyName();
    }
}
