using Microsoft.Extensions.Logging;

using StalkerBelarus.Launcher.Core.Helpers;
using StalkerBelarus.Launcher.Core.Models;
using StalkerBelarus.Launcher.Core.Services;
using StalkerBelarus.Launcher.Core.Storage;

namespace StalkerBelarus.Launcher.Core.Manager;

public class InitializerManager {
    private readonly ILogger<InitializerManager> _logger;
    private readonly IGitStorageApiService _gitStorageApiService;
    private readonly UserManager _userManager;
    private readonly ILocaleManager _localeManager;
    private readonly IReleaseComparerService<GitHubRelease> _releaseComparerService;

    public InitializerManager(ILogger<InitializerManager> logger, IGitStorageApiService gitStorageApiService, UserManager userManager,
        ILocaleManager localeManager, IReleaseComparerService<GitHubRelease> releaseComparerService) {
        _logger = logger;
        _gitStorageApiService = gitStorageApiService;
        _userManager = userManager;
        _localeManager = localeManager;
        _releaseComparerService = releaseComparerService;
    }

    public void Initialize() {
        SetLocale();
    }

    public async Task<bool> IsGameReleaseCurrentAsync() {
        var gitStorageRelease = await _gitStorageApiService.GetLastReleaseAsync();

        if (File.Exists(FileLocations.CurrentRelease)) {
            var releaseComparer = gitStorageRelease != null && await _releaseComparerService.IsComparerAsync(gitStorageRelease);
            if (!releaseComparer) {
                await FileSystemHelper.WriteReleaseAsync(gitStorageRelease, FileLocations.CurrentRelease);
                _logger.LogInformation("The releases don't match. Update required!");
                return false;
            } else {
                _logger.LogInformation("The releases are the same. No update required.");
                return true;
            }
        } else {
            await FileSystemHelper.WriteReleaseAsync(gitStorageRelease, FileLocations.CurrentRelease);
            _logger.LogInformation("The release configuration has not been previously saved");
            return false;
        }
    }

    private void SetLocale() {
        var userSettings = _userManager.UserSettings ??
            throw new Exception("Error loading user config!");
        _localeManager.SetLocale(userSettings.Locale);
    }
}
