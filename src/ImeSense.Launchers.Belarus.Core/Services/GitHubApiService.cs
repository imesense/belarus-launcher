using System.Net.Http.Json;
using System.Text.Json;

using ImeSense.Launchers.Belarus.Core.Models;
using ImeSense.Launchers.Belarus.Core.Storage;

using Microsoft.Extensions.Logging;

namespace ImeSense.Launchers.Belarus.Core.Services;

public class GitHubApiService(ILogger<GitHubApiService>? logger, HttpClient httpClient, ILauncherStorage? launcherStorage) : IGitStorageApiService
{
    private readonly ILogger<GitHubApiService>? _logger = logger;
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILauncherStorage? _launcherStorage = launcherStorage;

    /// <summary>
    /// Downloads a JSON file from a GitHub release and deserializes it into the specified object type
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize the JSON into</typeparam>
    /// <param name="filename">The name of the JSON file to download</param>
    /// <returns>The deserialized object of type T if successful, or null if the file is not found or deserialization fails</returns>
    public async Task<T?> DownloadJsonAsync<T>(string filename, Uri? uriRepository = null, CancellationToken cancellationToken = default) where T : class
    {
        if (uriRepository == null) {
            if (_httpClient.BaseAddress == null) {
                throw new NullReferenceException("No base address for HttpClient");
            }
            uriRepository = _httpClient.BaseAddress;
        }

        // Get the GitHub release information
        GitHubRelease? release;
        if (_launcherStorage == null) {
            release = await GetLastReleaseAsync(_httpClient.BaseAddress, cancellationToken: cancellationToken);
        } else {
            release = _launcherStorage.GitHubRelease;
        }
        // Find the asset with the specified filename
        var asset = release?.Assets?.FirstOrDefault(n => n.Name.Equals(filename));
        // Download the asset
        return await _httpClient.GetFromJsonAsync(asset?.BrowserDownloadUrl, typeof(T),
            SourceGenerationContext.Default, cancellationToken: cancellationToken) as T;
    }

    public async Task<GitHubRelease?> GetLastReleaseAsync(Uri? uriRepository = null, CancellationToken cancellationToken = default)
    {
        if (uriRepository == null) {
            if (_httpClient.BaseAddress == null) {
                throw new NullReferenceException("No base address for HttpClient");
            }
            uriRepository = _httpClient.BaseAddress;
        }

        return await _httpClient.GetFromJsonAsync(new Uri(uriRepository, "releases/latest"),
            SourceGenerationContext.Default.GitHubRelease, cancellationToken: cancellationToken);
    }

    public async Task<GitHubRelease?> GetReleaseAsync(string tag, Uri? uriRepository = null, CancellationToken cancellationToken = default)
    {
        if (uriRepository == null) {
            if (_httpClient.BaseAddress == null) {
                throw new NullReferenceException("No base address for HttpClient");
            }
            uriRepository = _httpClient.BaseAddress;
        }

        var json = await _httpClient.GetFromJsonAsync(new Uri(uriRepository, $"releases"),
            SourceGenerationContext.Default.GitHubReleaseArray, cancellationToken: cancellationToken);
        return json?.FirstOrDefault(t => t.TagName.Equals(tag));
    }

    public async Task<IEnumerable<Tag?>?> GetTagsAsync(Uri? uriRepository = null, CancellationToken cancellationToken = default)
    {
        if (uriRepository == null) {
            if (_httpClient.BaseAddress == null) {
                throw new NullReferenceException("No base address for HttpClient");
            }
            uriRepository = _httpClient.BaseAddress;
        }

        await using var tagsStream = await _httpClient.GetStreamAsync(new Uri(uriRepository, "tags"), cancellationToken);
        var tags = await JsonSerializer.DeserializeAsync(tagsStream,
            SourceGenerationContext.Default.IEnumerableTag, cancellationToken: cancellationToken);

        if (tags is not null) {
            return tags;
        } else {
            _logger?.LogError("Tags not found");
            return default;
        }
    }
}
