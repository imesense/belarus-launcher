using System.Diagnostics;
using System.Net.Http.Headers;

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
    public bool IsUserAuthorized { get; private set; }

    public InitializerManager(ILogger<InitializerManager> logger, IGitStorageApiService gitStorageApiService, UserManager userManager,
        ILocaleManager localeManager, ILauncherStorage launcherStorage, IReleaseComparerService<GitHubRelease> releaseComparerService) {
        _logger = logger;
        _gitStorageApiService = gitStorageApiService;
        _userManager = userManager;
        _localeManager = localeManager;
        _launcherStorage = launcherStorage;
        _releaseComparerService = releaseComparerService;
    }

    public async Task InitializeAsync() {
        SetLocale();
        try {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            _launcherStorage.GitHubRelease = await _gitStorageApiService.GetLastReleaseAsync();
            Task<IEnumerable<LangNewsContent>?> task3;
            IsUserAuthorized = File.Exists(FileLocations.UserSettingPath);
            if (IsUserAuthorized) {
                var locale = _userManager?.UserSettings?.Locale;
                task3 = LoadNewsAsync(locale);
            } else {
                task3 = LoadNewsAsync();
            }

            var task1 = IsGameReleaseCurrentAsync();
            var task2 = LoadWebResourcesAsync();

            await Task.WhenAll(task1, task2, task3);

            IsGameReleaseCurrent = task1.Result;
            _launcherStorage.WebResources = task2.Result;
            _launcherStorage.NewsContents = task3.Result;

            stopwatch.Stop();
            _logger.LogInformation("Parsing time: {Time}", stopwatch.ElapsedMilliseconds);
        } catch (Exception ex) {
            _logger.LogError("{Message}", ex.Message);
            _logger.LogError("{StackTrace}", ex.StackTrace);
        }
    }

    public static async Task<bool> IsLauncherReleaseCurrentAsync() {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("https://api.github.com/repos/imesense/belarus-launcher/");
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
        httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

        var gitHubService = new GitHubApiService(null, httpClient, null);
        var tags = await gitHubService.GetTagsAsync();
        if (tags != null) {
            var firstTag = tags.FirstOrDefault();
            if (firstTag != null) {
                var currentVersion = ApplicationHelper.GetAppVersion();
                return firstTag.Name.Equals(currentVersion);
            }
            return true;
        }
        return true;
    }

    private async Task<bool> IsGameReleaseCurrentAsync() {
        var gitStorageRelease = _launcherStorage.GitHubRelease;

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

    private async Task<IEnumerable<LangNewsContent>?> LoadNewsAsync(Locale? locale = null) {
        // News in all languages
        var allNews = new List<LangNewsContent>();

        try {
            if (locale is null) {
                foreach (var lang in _launcherStorage.Locales) {
                    var news = await _gitStorageApiService.DownloadJsonAsync<IEnumerable<NewsContent>>($"news_content_{lang.Key}.json");
                    AddNews(lang, allNews, news);
                }
            } else {
                var news = await _gitStorageApiService.DownloadJsonAsync<IEnumerable<NewsContent>>($"news_content_{locale.Key}.json");
                AddNews(locale, allNews, news);
            }
        } catch (Exception ex) {
            _logger.LogError("{Message}", ex.Message);
            _logger.LogError("{StackTrace}", ex.StackTrace);
        }

        return allNews;
    }

    private void AddNews(Locale? locale, List<LangNewsContent> allNews, IEnumerable<NewsContent>? news) {
        if (locale is null) {
            _logger.LogError("Failure to load locale!");
            return;
        }

        if (news != null) {
            allNews.Add(new LangNewsContent(locale, news));
        } else {
            _logger.LogError("Failure to load news in {locale}", locale.Title);
        }
    }
}
