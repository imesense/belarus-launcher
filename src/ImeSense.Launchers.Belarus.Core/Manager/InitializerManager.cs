using System.Diagnostics;

using ImeSense.Launchers.Belarus.Core.Helpers;
using ImeSense.Launchers.Belarus.Core.Models;
using ImeSense.Launchers.Belarus.Core.Services;
using ImeSense.Launchers.Belarus.Core.Storage;

using Microsoft.Extensions.Logging;

namespace ImeSense.Launchers.Belarus.Core.Manager;

public class InitializerManager(
    ILogger<InitializerManager> logger,
    IGitStorageApiService gitStorageApiService, UserManager userManager,
    ILocaleManager localeManager, ILauncherStorage launcherStorage,
    IReleaseComparerService<GitHubRelease> releaseComparerService,
    IUpdaterService updaterService)
{
    private readonly ILogger<InitializerManager> _logger = logger;
    private readonly IGitStorageApiService _gitStorageApiService = gitStorageApiService;
    private readonly UserManager _userManager = userManager;
    private readonly ILocaleManager _localeManager = localeManager;
    private readonly ILauncherStorage _launcherStorage = launcherStorage;
    private readonly IReleaseComparerService<GitHubRelease> _releaseComparerService = releaseComparerService;
    private readonly IUpdaterService _updaterService = updaterService;

    public bool IsGameReleaseCurrent { get; private set; } = true;
    public bool IsUserAuthorized { get; private set; }

    public async Task InitializeAsync()
    {
        SetLocale();

        try {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            _launcherStorage.IsCheckGitHubConnection = await CheckGitHubConnectionAsync();
            var locale = _userManager?.UserSettings?.Locale;

            if (_launcherStorage.IsCheckGitHubConnection) {
                try {
                    var isLauncherReleaseCurrent = await IsLauncherReleaseCurrentAsync();
                    if (!isLauncherReleaseCurrent) {
                        var pathLauncherUpdater = Path.Combine(DirectoryStorage.Base,
                            FileNameStorage.SBLauncherUpdater);
                        await _updaterService.UpdaterAsync(UriStorage.LauncherApiUri, pathLauncherUpdater);

                        var updater = Launcher.Launch(pathLauncherUpdater);
                        updater?.Start();

                        return;
                    }
                } catch (Exception ex) {
                    _logger.LogError("{Message}", ex.Message);
                    _logger.LogError("{StackTrace}", ex.StackTrace);
                }

                _launcherStorage.GitHubRelease = await _gitStorageApiService.GetLastReleaseAsync();
                IsGameReleaseCurrent = await IsGameReleaseCurrentAsync();
                IsUserAuthorized = File.Exists(PathStorage.LauncherSetting);

                if (IsUserAuthorized) {
                    _launcherStorage.NewsContents = await LoadNewsAsync(locale);
                } else {
                    _launcherStorage.NewsContents = await LoadNewsAsync();
                }
                _launcherStorage.WebResources = await LoadWebResourcesAsync();
            } else {
                if (IsUserAuthorized) {
                    _launcherStorage.NewsContents = LoadErrorNews(locale);
                } else {
                    _launcherStorage.NewsContents = LoadErrorNews();
                }
                
            }

            stopwatch.Stop();
            _logger.LogInformation("Parsing time: {Time}", stopwatch.ElapsedMilliseconds);
        } catch (Exception ex) {
            _logger.LogError("{Message}", ex.Message);
            _logger.LogError("{StackTrace}", ex.StackTrace);
        }
    }

    private IList<LangNewsContent>? LoadErrorNews(Locale? locale = null)
    {
        // News in all languages
        var allNews = new List<LangNewsContent>();
        locale ??= _launcherStorage.Locales[0];

        try {
            allNews.Add(new LangNewsContent(locale, [new NewsContent(
                _localeManager.GetStringByKey("LocalizedStrings.ErrorTitle", locale.Key),
                _localeManager.GetStringByKey("LocalizedStrings.ErrorInternetDescription", locale.Key)
            )]));
        } catch (Exception ex) {
            _logger.LogError("{Message}", ex.Message);
            _logger.LogError("{StackTrace}", ex.StackTrace);
        }

        return allNews;
    }


    /// <summary>
    /// Checks the connection to github.com.
    /// </summary>
    /// <returns>True if the connection is established successfully, otherwise false.</returns>
    private async Task<bool> CheckGitHubConnectionAsync()
    {
        try {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("https://github.com");

            if (response.IsSuccessStatusCode) {
                _logger.LogInformation("Connection to github.com established");
                return true;
            } else {
                _logger.LogInformation("Failed to establish connection to github.com. Response code: {StatusCode}", response.StatusCode);
                return false;
            }
        } catch (HttpRequestException ex) {
            _logger.LogInformation("Failed to establish connection to github.com");

            _logger.LogError("{Message}", ex.Message);
            _logger.LogError("{StackTrace}", ex.StackTrace);
            return false;
        }
    }


    private async Task<bool> IsLauncherReleaseCurrentAsync()
    {
        var tags = await _gitStorageApiService.GetTagsAsync(UriStorage.LauncherApiUri);
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

    private async Task<bool> IsGameReleaseCurrentAsync()
    {
        var gitStorageRelease = _launcherStorage.GitHubRelease;

        if (File.Exists(PathStorage.CurrentRelease)) {
            var releaseComparer = gitStorageRelease != null && await _releaseComparerService.IsComparerAsync(gitStorageRelease);
            if (!releaseComparer) {
                await FileSystemHelper.WriteReleaseAsync(gitStorageRelease, PathStorage.CurrentRelease);
                _logger.LogInformation("The releases don't match. Update required!");
                return false;
            } else {
                _logger.LogInformation("The releases are the same. No update required.");
                return true;
            }
        } else {
            await FileSystemHelper.WriteReleaseAsync(gitStorageRelease, PathStorage.CurrentRelease);
            _logger.LogInformation("The release configuration has not been previously saved");
            return false;
        }
    }

    private void SetLocale()
    {
        var userSettings = _userManager.UserSettings ??
            throw new Exception("Error loading user config!");

        userSettings.Locale = new();

        if (userSettings.Locale.Key == string.Empty) {
            var defaultLocale = _launcherStorage.Locales[0];
            userSettings.Locale = defaultLocale;
        }

        _localeManager.SetLocale(userSettings.Locale.Key);
    }

    private async Task<IEnumerable<WebResource>> LoadWebResourcesAsync()
    {
        try {
            var contents = await _gitStorageApiService
                .DownloadJsonAsync<IEnumerable<WebResource>>(FileNameStorage.WebResources, UriStorage.BelarusApiUri);
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

    private async Task<IList<LangNewsContent>?> LoadNewsAsync(Locale? locale = null)
    {
        // News in all languages
        var allNews = new List<LangNewsContent>();

        try {
            if (locale is null) {
                foreach (var lang in _launcherStorage.Locales) {
                    var news = await _gitStorageApiService
                        .DownloadJsonAsync<IEnumerable<NewsContent>>($"news_content_{lang.Key}.json", UriStorage.BelarusApiUri);
                    AddNews(lang, allNews, news);
                }
            } else {
                var news = await _gitStorageApiService
                    .DownloadJsonAsync<IEnumerable<NewsContent>>($"news_content_{locale.Key}.json", UriStorage.BelarusApiUri);
                AddNews(locale, allNews, news);
            }
        } catch (Exception ex) {
            _logger.LogError("{Message}", ex.Message);
            _logger.LogError("{StackTrace}", ex.StackTrace);
        }

        return allNews;
    }

    private void AddNews(Locale? locale, List<LangNewsContent> allNews, IEnumerable<NewsContent>? news)
    {
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
