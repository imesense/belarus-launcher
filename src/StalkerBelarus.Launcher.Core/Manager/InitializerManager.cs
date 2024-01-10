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
    private readonly ILauncherStorage _launcherStorage;
    private readonly IReleaseComparerService<GitHubRelease> _releaseComparerService;

    public bool IsGameReleaseCurrent { get; private set; }

    public InitializerManager(ILogger<InitializerManager> logger, IGitStorageApiService gitStorageApiService, UserManager userManager,
        ILocaleManager localeManager, ILauncherStorage launcherStorage, IReleaseComparerService<GitHubRelease> releaseComparerService) {
        _logger = logger;
        _gitStorageApiService = gitStorageApiService;
        _userManager = userManager;
        _localeManager = localeManager;
        _launcherStorage = launcherStorage;
        _releaseComparerService = releaseComparerService;
    }

    public async Task<bool> InitializeAsync() {
        SetLocale();
        try {
            IsGameReleaseCurrent = await IsGameReleaseCurrentAsync();
            _launcherStorage.WebResources = await LoadWebResourcesAsync();
            _launcherStorage.NewsContents = await LoadNewsAsync();
        } catch (Exception ex) {
            _logger.LogError("{Message}", ex.Message);
            _logger.LogError("{StackTrace}", ex.StackTrace);
        }

        
        return true;
    }

    private async Task<bool> IsGameReleaseCurrentAsync() {
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
        if (userSettings.Locale is null) {
            throw new NullReferenceException("User settings locale object is null");
        }

        if (userSettings.Locale.Key == string.Empty) {
            var defaultLocale = _launcherStorage.Locales[0];
            userSettings.Locale = defaultLocale;
        }

        _localeManager.SetLocale(userSettings.Locale.Key);
    }

    private async Task<IEnumerable<WebResource>> LoadWebResourcesAsync() {
        try {
            var contents = await _gitStorageApiService.DownloadJsonAsync<IEnumerable<WebResource>>(FileNamesStorage.WebResources);
            var webResources = new List<WebResource>();

            if (contents != null) {
                foreach (var content in contents) {
                    if (content != null) {
                        webResources.Add(content);
                    }
                }
            }

            return webResources;
        } catch (Exception ex) {
            _logger.LogError("{Message}", ex.Message);
            _logger.LogError("{StackTrace}", ex.StackTrace);
            throw;
        }

    }

    public async Task<IEnumerable<LangNewsContent>?> LoadNewsAsync() {
        // News in all languages
        var allNews = new List<LangNewsContent>();

        try {
            foreach (var locale in _launcherStorage.Locales) {
                var news = await _gitStorageApiService.DownloadJsonAsync<IEnumerable<NewsContent>>($"news_content_{locale.Key}.json");

                if (news != null) {
                    allNews.Add(new LangNewsContent(locale, news));
                } else {
                    _logger.LogError("Failure to load news in {locale}", locale.Title);
                }
            }
        } catch (Exception ex) {
            _logger.LogError("{Message}", ex.Message);
            _logger.LogError("{StackTrace}", ex.StackTrace);
        }

        return allNews;
    }
}
