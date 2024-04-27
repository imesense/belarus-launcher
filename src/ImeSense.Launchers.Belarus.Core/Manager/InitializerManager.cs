using System.Diagnostics;

using ImeSense.Launchers.Belarus.Core.Helpers;
using ImeSense.Launchers.Belarus.Core.Models;
using ImeSense.Launchers.Belarus.Core.Services;
using ImeSense.Launchers.Belarus.Core.Storage;
using ImeSense.Launchers.Belarus.Models;

using Microsoft.Extensions.Logging;

namespace ImeSense.Launchers.Belarus.Core.Manager;

public class InitializerManager(
    ILogger<InitializerManager>? logger,
    HttpClient httpClient,
    IGitStorageApiService gitStorageApiService, UserManager userManager,
    IApplicationLocaleManager localeManager, ILauncherStorage launcherStorage,
    IReleaseComparerService<GitHubRelease> releaseComparerService,
    IUpdaterService updaterService)
{
    private readonly ILogger<InitializerManager>? _logger = logger;
    private readonly HttpClient _httpClient = httpClient;
    private readonly IGitStorageApiService _gitStorageApiService = gitStorageApiService;
    private readonly UserManager _userManager = userManager;
    private readonly IApplicationLocaleManager _localeManager = localeManager;
    private readonly ILauncherStorage _launcherStorage = launcherStorage;
    private readonly IReleaseComparerService<GitHubRelease> _releaseComparerService = releaseComparerService;
    private readonly IUpdaterService _updaterService = updaterService;

    public async Task InitializeAsync(ISplashScreenManager splashScreenManager)
    {
        try {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            if (_userManager is null) {
                throw new NullReferenceException("User manager object is null");
            }
            if (_userManager.UserSettings is null) {
                throw new NullReferenceException("User settings object is null");
            }
            if (_userManager.UserSettings.Locale is null) {
                _logger?.LogError("User settings locale object is null. Default locale will be selected");
                _userManager.UserSettings.Locale = _launcherStorage.Locales[0];
            }
            var locale = _userManager.UserSettings.Locale;

            if (!Directory.Exists(DirectoryStorage.LauncherCache)) {
                Directory.CreateDirectory(DirectoryStorage.LauncherCache);
            }

            splashScreenManager.UpdateInformation(new InformationMessage(
                _localeManager.GetStringByKey("LocalizedStrings.Loading"),
                _localeManager.GetStringByKey("LocalizedStrings.AccessingRepository")));

            _launcherStorage.IsCheckGitHubConnection = await CheckGitHubConnectionAsync(splashScreenManager.CancellationToken);
            _logger?.LogInformation("Check GitHub connection time: {Time}", stopwatch.ElapsedMilliseconds);

            if (_launcherStorage.IsCheckGitHubConnection) {
                var isLauncherReleaseCurrent = await IsLauncherReleaseCurrentAsync(splashScreenManager.CancellationToken);
                _logger?.LogInformation("Check launcher update time: {Time}", stopwatch.ElapsedMilliseconds);
                if (!isLauncherReleaseCurrent) {
                    var pathLauncherUpdater = Path.Combine(DirectoryStorage.Base,
                        FileNameStorage.SBLauncherUpdater);
                    await _updaterService.UpdaterAsync(UriStorage.LauncherApiUri, pathLauncherUpdater, splashScreenManager.CancellationToken);

                    var updater = Launcher.Launch(pathLauncherUpdater);
                    updater?.Start();

                    return;
                }

                _launcherStorage.GitHubRelease = await _gitStorageApiService.GetLastReleaseAsync(cancellationToken: splashScreenManager.CancellationToken);
                _logger?.LogInformation("Check last release time: {Time}", stopwatch.ElapsedMilliseconds);

                _launcherStorage.IsGameReleaseCurrent = await IsGameReleaseCurrentAsync(splashScreenManager.CancellationToken);
                _launcherStorage.IsUserAuthorized = File.Exists(PathStorage.LauncherSetting);

                if (_launcherStorage.IsGameReleaseCurrent) {
                    var contentNews = await LocaleLoadCacheAsync<LangNewsContent>(PathStorage.NewsCache) ?? [];
                    var isContentNews = contentNews.Any(x => x.Locale is not null && x.Locale.Key.Equals(_userManager.UserSettings.Locale.Key));
                    if (isContentNews) {
                        _launcherStorage.NewsContents = new(contentNews);
                    } else {
                        await LoadRemoteContent(splashScreenManager, locale);
                    }

                    var contentRes = await LocaleLoadCacheAsync<WebResource>(PathStorage.WebResourcesCache);
                    if (contentRes is not null && contentRes.Count != 0) {
                        _launcherStorage.WebResources = new(contentRes);
                    } else {
                        await Task.Factory.StartNew(() => RemoteLoadWebResourcesAsync(cancellationToken: splashScreenManager.CancellationToken));
                    }
                } else {
                    await LoadRemoteContent(splashScreenManager, locale);
                    await Task.Factory.StartNew(() => RemoteLoadWebResourcesAsync(cancellationToken: splashScreenManager.CancellationToken));
                }
            } else {
                if (_launcherStorage.IsUserAuthorized) {
                    _launcherStorage.NewsContents = new(LoadErrorNews(locale) ?? []);
                } else {
                    _launcherStorage.NewsContents = new(LoadErrorNews() ?? []);
                }

                _launcherStorage.GitHubRelease = await FileDataHelper.LoadDataAsync<GitHubRelease>(PathStorage.CurrentRelease, splashScreenManager.CancellationToken);
            }

            stopwatch.Stop();
            _logger?.LogInformation("Parsing time: {Time}", stopwatch.ElapsedMilliseconds);
        } catch (Exception ex) {
            _logger?.LogError("{Message}", ex.Message);
            _logger?.LogError("{StackTrace}", ex.StackTrace);
            throw;
        }
    }

    private async Task LoadRemoteContent(ISplashScreenManager splashScreenManager, Locale? locale)
    {
        if (_launcherStorage.IsUserAuthorized) {
            await Task.Factory.StartNew(() => RemoteLoadNewsAsync(locale, splashScreenManager.CancellationToken), splashScreenManager.CancellationToken);
        } else {
            await Task.Factory.StartNew(() => RemoteLoadNewsAsync(cancellationToken: splashScreenManager.CancellationToken), splashScreenManager.CancellationToken);
        }
    }

    private async Task<List<T>?> LocaleLoadCacheAsync<T>(string cachePath, CancellationToken cancellationToken = default)
    {
        try {
            if (!File.Exists(cachePath)) {
                return null;
            }

            return await FileDataHelper.LoadDataAsync<List<T>>(cachePath, cancellationToken);
        } catch (Exception ex) {
            _logger?.LogError("{Message}", ex.Message);
            _logger?.LogError("{StackTrace}", ex.StackTrace);
            throw;
        }
    }

    private List<LangNewsContent>? LoadErrorNews(Locale? locale = null)
    {
        // News in all languages
        var allNews = new List<LangNewsContent>();
        locale ??= _launcherStorage.Locales[0];

        try {
            allNews.Add(new LangNewsContent(locale, [new NewsContent(
                _localeManager.GetStringByKey("LocalizedStrings.ErrorTitle"),
                _localeManager.GetStringByKey("LocalizedStrings.ErrorInternetDescription")
            )]));
        } catch (Exception ex) {
            _logger?.LogError("{Message}", ex.Message);
            _logger?.LogError("{StackTrace}", ex.StackTrace);
        }

        return allNews;
    }


    /// <summary>
    /// Checks the connection to github.com.
    /// </summary>
    /// <returns>True if the connection is established successfully, otherwise false.</returns>
    private async Task<bool> CheckGitHubConnectionAsync(CancellationToken cancellationToken = default)
    {
        try {
            var response = await _httpClient.GetAsync("https://github.com", cancellationToken);

            if (response.IsSuccessStatusCode) {
                _logger?.LogInformation("Connection to github.com established");
                return true;
            } else {
                _logger?.LogInformation("Failed to establish connection to github.com. Response code: {StatusCode}", response.StatusCode);
                return false;
            }
        } catch (HttpRequestException ex) {
            _logger?.LogInformation("Failed to establish connection to github.com");

            _logger?.LogError("{Message}", ex.Message);
            _logger?.LogError("{StackTrace}", ex.StackTrace);
            return false;
        }
    }

    private async Task<bool> IsLauncherReleaseCurrentAsync(CancellationToken cancellationToken = default)
    {
        var tags = await _gitStorageApiService.GetTagsAsync(UriStorage.LauncherApiUri, cancellationToken);
        if (tags != null) {
            var currentVersion = $"{ApplicationHelper.GetAppVersion()}";
            if (currentVersion[0] != 'v') {
                currentVersion = currentVersion.Insert(0, "v");
            }

            var countTag = tags.Count(x => x!.Name.Equals(currentVersion));
            if (countTag == 0) {
                // If there is no such release, we return true so that there is no looping
                return true;
            }

            var firstTag = tags.FirstOrDefault();
            if (firstTag != null) {
                return firstTag.Name.Equals(currentVersion);
            }
            return true;
        }
        return true;
    }

    private async Task<bool> IsGameReleaseCurrentAsync(CancellationToken cancellationToken = default)
    {
        var gitStorageRelease = _launcherStorage.GitHubRelease;

        if (File.Exists(PathStorage.CurrentRelease)) {
            var releaseComparer = gitStorageRelease != null && await _releaseComparerService.IsComparerAsync(gitStorageRelease, cancellationToken);
            if (!releaseComparer) {
                await FileSystemHelper.WriteReleaseAsync(gitStorageRelease, PathStorage.CurrentRelease, cancellationToken);
                _logger?.LogInformation("The releases don't match. Update required!");
                return false;
            } else {
                _logger?.LogInformation("The releases are the same. No update required.");
                return true;
            }
        } else {
            await FileSystemHelper.WriteReleaseAsync(gitStorageRelease, PathStorage.CurrentRelease, cancellationToken);
            _logger?.LogInformation("The release configuration has not been previously saved");
            return false;
        }
    }

    public void InitializeLocale()
    {
        var userSettings = _userManager.UserSettings ??
            throw new Exception("Error loading user config!");

        if (userSettings.Locale is null) {
            _logger?.LogError("Locale was not set");
            userSettings.Locale = _launcherStorage.Locales[0];
        }

        _localeManager.SetLocale(userSettings.Locale.Key);
    }

    private async Task RemoteLoadWebResourcesAsync(CancellationToken cancellationToken = default)
    {
        try {
            var contents = await _gitStorageApiService
                .DownloadJsonAsync<IEnumerable<WebResource>>(FileNameStorage.WebResources, UriStorage.BelarusApiUri, cancellationToken);
            if (contents != null) {
                await FileSystemHelper.WriteReleaseAsync(contents, Path.Combine(DirectoryStorage.LauncherCache, FileNameStorage.WebResources), cancellationToken);
                _launcherStorage.WebResources = new(contents);
            }
        } catch (Exception ex) {
            _logger?.LogError("{Message}", ex.Message);
            _logger?.LogError("{StackTrace}", ex.StackTrace);
            throw;
        }
    }

    private async Task RemoteLoadNewsAsync(Locale? locale = null, CancellationToken cancellationToken = default)
    {
        // News in all languages
        var allNews = new List<LangNewsContent>();

        try {
            if (locale is null) {
                foreach (var lang in _launcherStorage.Locales) {
                    var news = await _gitStorageApiService
                        .DownloadJsonAsync<IEnumerable<NewsContent>>($"news_content_{lang.Key}.json", UriStorage.BelarusApiUri, cancellationToken);
                    AddNews(lang, allNews, news);
                }
            } else {
                var news = await _gitStorageApiService
                    .DownloadJsonAsync<IEnumerable<NewsContent>>($"news_content_{locale.Key}.json", UriStorage.BelarusApiUri, cancellationToken);
                AddNews(locale, allNews, news);
            }
        } catch (Exception ex) {
            _logger?.LogError("{Message}", ex.Message);
            _logger?.LogError("{StackTrace}", ex.StackTrace);
        }

        await FileSystemHelper.WriteReleaseAsync(allNews, Path.Combine(DirectoryStorage.LauncherCache, "News.json"), cancellationToken);
        _launcherStorage.NewsContents = new(allNews);
    }

    private void AddNews(Locale? locale, List<LangNewsContent> allNews, IEnumerable<NewsContent>? news)
    {
        if (locale is null) {
            _logger?.LogError("Failure to load locale!");
            return;
        }

        if (news != null) {
            allNews.Add(new LangNewsContent(locale, news));
        } else {
            _logger?.LogError("Failure to load news in {locale}", locale.Title);
        }
    }
}
