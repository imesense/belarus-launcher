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
        uriRepository ??= _httpClient.BaseAddress ?? throw new NullReferenceException("No base address for HttpClient");

        // Get the GitHub release information
        GitHubRelease? release;
        if (_launcherStorage is null)
        {
            release = await GetLastReleaseAsync(_httpClient.BaseAddress, cancellationToken: cancellationToken);
            _logger?.LogInformation("Retrieved last release information from server");
        }
        else
        {
            release = _launcherStorage.GitHubRelease;
            _logger?.LogInformation("Retrieved last release information from local storage");
        }

        if (release is null || release.Assets is null)
        {
            _logger?.LogWarning("Release information or assets are null");
            return null;
        }

        // Find the asset with the specified filename
        var asset = release.Assets.FirstOrDefault(n => n.Name.Equals(filename));
        if (asset is null)
        {
            _logger?.LogError("Asset with filename {Filename} not found", filename);
            return null;
        }

        // Download the asset
        var response = await _httpClient.GetAsync(asset.BrowserDownloadUrl, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger?.LogError("Failed to download asset from {BrowserDownloadUrl}. Status code: {StatusCode}",
                asset.BrowserDownloadUrl, response.StatusCode);
            return null;
        }
        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        return await JsonSerializer.DeserializeAsync(stream, typeof(T), SourceGenerationContext.Default, cancellationToken) as T;
    }

    public async Task<GitHubRelease?> GetLastReleaseAsync(Uri? uriRepository = null, CancellationToken cancellationToken = default)
    {
        uriRepository ??= _httpClient.BaseAddress ?? throw new NullReferenceException("No base address for HttpClient");

        var response = await _httpClient.GetAsync(new Uri(uriRepository, "releases/latest"), cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger?.LogError("Failed to get last release from {UriRepository}. Status code: {StatusCode}", uriRepository,
                response.StatusCode);
            return null;
        }

        return await response.Content.ReadFromJsonAsync(typeof(GitHubRelease), SourceGenerationContext.Default, cancellationToken: cancellationToken) as GitHubRelease;
    }

    public async Task<GitHubRelease?> GetReleaseAsync(string tag, Uri? uriRepository = null, CancellationToken cancellationToken = default)
    {
        uriRepository ??= _httpClient.BaseAddress ?? throw new NullReferenceException("No base address for HttpClient");

        var response = await _httpClient.GetAsync(new Uri(uriRepository, $"releases"), cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger?.LogError("Failed to get releases from {UriRepository}. Status code: {StatusCode}", uriRepository,
                response.StatusCode);
            return null;
        }

        var releases = await response.Content.ReadFromJsonAsync(typeof(List<GitHubRelease>), SourceGenerationContext.Default, cancellationToken: cancellationToken) as List<GitHubRelease>;

        return releases?.FirstOrDefault(t => t.TagName.Equals(tag));
    }

    public async Task<IEnumerable<Tag?>?> GetTagsAsync(Uri? uriRepository = null, CancellationToken cancellationToken = default)
    {
        uriRepository ??= _httpClient.BaseAddress ?? throw new NullReferenceException("No base address for HttpClient");
        var response = await _httpClient.GetAsync(new Uri(uriRepository, "tags"), cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger?.LogError("Failed to get tags from {UriRepository}. Status code: {StatusCode}",
               uriRepository, response.StatusCode);
            return default;
        }
        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var tags = await JsonSerializer.DeserializeAsync(stream, SourceGenerationContext.Default.IEnumerableTag, cancellationToken: cancellationToken);

        if (tags is not null)
        {
            return tags;
        }
        else
        {
            _logger?.LogError("Tags not found");
            return default;
        }
    }
}
