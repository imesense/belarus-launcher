using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using StalkerBelarus.Launcher.Avalonia.Helpers;
using StalkerBelarus.Launcher.Core.Validators;

namespace StalkerBelarus.Launcher.Avalonia.ViewModels;

public class LauncherViewModel : ReactiveObject {
    private readonly ILogger<LauncherViewModel> _logger;
    private readonly DownloadMenuViewModel _downloadMenuViewModel;
    private readonly GameMenuViewModel _gameMenuViewModel;
    private readonly GameDirectoryValidator _directoryValidator;

    public string AppVersion { get; set; }
    public string CompanyName { get; set; }

    [Reactive] public ReactiveObject PageMenuViewModel { get; set; } = null!;
    [Reactive] public NewsSliderViewModel NewsSliderViewModel { get; set; }


    public LauncherViewModel(ILogger<LauncherViewModel> logger, DownloadMenuViewModel downloadMenuViewModel, 
        GameMenuViewModel gameMenuViewModel, NewsSliderViewModel newsSliderViewModel,
        GameDirectoryValidator directoryValidator) {
        _logger = logger;

        _logger.LogInformation("LauncherViewModel CTOR");
        _downloadMenuViewModel = downloadMenuViewModel;
        _gameMenuViewModel = gameMenuViewModel;
        _directoryValidator = directoryValidator;
        NewsSliderViewModel = newsSliderViewModel;
        
        AppVersion = ApplicationHelper.GetAppVersion();
        CompanyName = (char) 0169 + ApplicationHelper.GetCompanyName();
    }

#if DEBUG
    public LauncherViewModel() {
        _logger = null!;
        _downloadMenuViewModel = null!;
        _gameMenuViewModel = null!;
        _directoryValidator = null!;

        AppVersion = null!;
        CompanyName = null!;

        NewsSliderViewModel = null!;
    }
#endif

    public void SelectMenu() {
        if (_directoryValidator.IsDirectoryValid()) {
            PageMenuViewModel = _gameMenuViewModel;
        } else {
            PageMenuViewModel = _downloadMenuViewModel;
        }
    }

    public async Task SelectUpdateMenuAsync() {
        PageMenuViewModel = _downloadMenuViewModel;
        await _downloadMenuViewModel.UpdateAsync(this);
    }
}
