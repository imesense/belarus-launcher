using Microsoft.Extensions.Logging;

using ReactiveUI.Fody.Helpers;

using StalkerBelarus.Launcher.Avalonia.Helpers;
using StalkerBelarus.Launcher.Core.Validators;

namespace StalkerBelarus.Launcher.Avalonia.ViewModels;

public class LauncherViewModel : ViewModelBase {
    private readonly ILogger<LauncherViewModel> _logger;
    private readonly DownloadMenuViewModel _downloadMenuViewModel;
    private readonly GameMenuViewModel _gameMenuViewModel;
    private readonly GameDirectoryValidator _directoryValidator;

    public string AppVersion { get; set; }
    public string CompanyName { get; set; }

    [Reactive] public ViewModelBase PageMenuViewModel { get; set; } = null!;
    [Reactive] public NewsSliderViewModel NewsSliderViewModel { get; set; }


    public LauncherViewModel(ILogger<LauncherViewModel> logger, DownloadMenuViewModel downloadMenuViewModel, 
        GameMenuViewModel gameMenuViewModel, NewsSliderViewModel newsSliderViewModel,
        GameDirectoryValidator directoryValidator) {
        _logger = logger;
        _downloadMenuViewModel = downloadMenuViewModel;
        _gameMenuViewModel = gameMenuViewModel;
        _directoryValidator = directoryValidator;
        NewsSliderViewModel = newsSliderViewModel;
        
        AppVersion = ApplicationHelper.GetAppVersion();
        CompanyName = (char) 0169 + ApplicationHelper.GetCompanyName();
    }

    public void SelectMenu() {
        if (_directoryValidator.IsDirectoryValid()) {
            PageMenuViewModel = _gameMenuViewModel;
        } else {
            PageMenuViewModel = _downloadMenuViewModel;
        }
    }
}
