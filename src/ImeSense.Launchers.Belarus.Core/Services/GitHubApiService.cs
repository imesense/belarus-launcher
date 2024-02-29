using System.Net.Http.Json;
using System.Text.Json;

using Microsoft.Extensions.Logging;

using ImeSense.Launchers.Belarus.Core.Models;
using ImeSense.Launchers.Belarus.Core.Storage;

namespace ImeSense.Launchers.Belarus.Core.Services;

public class GitHubApiService : IGitStorageApiService {
    private readonly ILogger<GitHubApiService>? _logger;
    private readonly HttpClient _httpClient;
    private readonly ILauncherStorage? _launcherStorage;

    public GitHubApiService(ILogger<GitHubApiService>? logger, HttpClient httpClient, ILauncherStorage? launcherStorage) {
        _logger = logger;
        _httpClient = httpClient;
        _launcherStorage = launcherStorage;
    }

    //public async IAsyncEnumerable<T?> DownloadJsonArrayAsync<T>(string filename, 
    //    Uri? uriRepository = null) where T : class
    //{
    //    if (uriRepository == null) {
    //        if (_httpClient.BaseAddress == null) {
    //            throw new NullReferenceException("No base address for HttpClient");
    //        }
    //        uriRepository = _httpClient.BaseAddress;
    //    }

    //    GitHubRelease? release;
    //    if (_launcherStorage == null) {
    //        release = await GetLastReleaseAsync();
    //    } else {
    //        release = _launcherStorage.GitHubRelease;
    //    }

    //    var asset = release?.Assets?.FirstOrDefault(n => n.Name.Equals(filename));
    //    await using var assetStream = await _httpClient.GetStreamAsync(asset?.BrowserDownloadUrl);
    //    var contents = JsonSerializer.DeserializeAsyncEnumerable<T>(assetStream);
    //    await foreach (var content in contents) {
    //        yield return content;
    //    }
    //}

    /// <summary>
    /// Downloads a JSON file from a GitHub release and deserializes it into the specified object type
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize the JSON into</typeparam>
    /// <param name="filename">The name of the JSON file to download</param>
    /// <returns>The deserialized object of type T if successful, or null if the file is not found or deserialization fails</returns>
    public async Task<T?> DownloadJsonAsync<T>(string filename, Uri? uriRepository = null) where T : class {
        if (uriRepository == null) {
            if (_httpClient.BaseAddress == null) {
                throw new NullReferenceException("No base address for HttpClient");
            }
            uriRepository = _httpClient.BaseAddress;
        }

        // Get the GitHub release information
        GitHubRelease? release;
        if (_launcherStorage == null) {
            release = await GetLastReleaseAsync();
        } else {
            release = _launcherStorage.GitHubRelease;
        }
        // Find the asset with the specified filename
        var asset = release?.Assets?.FirstOrDefault(n => n.Name.Equals(filename));
        // Download the asset
        return await _httpClient.GetFromJsonAsync(asset?.BrowserDownloadUrl, typeof(T), SourceGenerationContext.Default) as T;
    }

    public async Task<GitHubRelease?> GetLastReleaseAsync(Uri? uriRepository = null) {
        if (uriRepository == null) {
            if (_httpClient.BaseAddress == null) {
                throw new NullReferenceException("No base address for HttpClient");
            }
            uriRepository = _httpClient.BaseAddress;
        }

        return await _httpClient.GetFromJsonAsync(new Uri(uriRepository, "releases/latest"), SourceGenerationContext.Default.GitHubRelease);
    }

    public async Task<GitHubRelease?> GetReleaseAsync(string tag, Uri? uriRepository = null) {
        if (uriRepository == null) {
            if (_httpClient.BaseAddress == null) {
                throw new NullReferenceException("No base address for HttpClient");
            }
            uriRepository = _httpClient.BaseAddress;
        }

        var json = await _httpClient.GetFromJsonAsync(new Uri(uriRepository, $"releases"), SourceGenerationContext.Default.GitHubReleaseArray);
        return json?.FirstOrDefault(t => t.TagName.Equals(tag));
    }

    public async Task<IEnumerable<Tag?>?> GetTagsAsync(Uri? uriRepository = null) {
        if (uriRepository == null) {
            if (_httpClient.BaseAddress == null) {
                throw new NullReferenceException("No base address for HttpClient");
            }
            uriRepository = _httpClient.BaseAddress;
        }

        await using var tagsStream = await _httpClient.GetStreamAsync(new Uri(uriRepository, "tags"));
        var tags = JsonSerializer.Deserialize(tagsStream, SourceGenerationContext.Default.IEnumerableTag);

        if (tags is not null) {
            return tags;
        } else {
            _logger?.LogError("Tags not found");
            return default;
        }
    }
}
