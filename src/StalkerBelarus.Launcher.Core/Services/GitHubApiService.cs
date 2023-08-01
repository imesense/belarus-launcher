using System.Text.Json;

using Microsoft.Extensions.Logging;

using StalkerBelarus.Launcher.Core.Models;

namespace StalkerBelarus.Launcher.Core.Services;

public class GitHubApiService : IGitHubApiService {
    private readonly ILogger<GitHubApiService> _logger;
    private readonly HttpClient _httpClient;

    public GitHubApiService(ILogger<GitHubApiService> logger, HttpClient httpClient) {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async IAsyncEnumerable<T?> DownloadJsonArrayAsync<T>(string filename) where T : class {
        var release = await GetGitHubReleaseAsync();
        var asset = release?.Assets?.FirstOrDefault(n => n.Name.Equals(filename));
        await using var assetStream = await _httpClient.GetStreamAsync(asset?.BrowserDownloadUrl);
        var contents = JsonSerializer.DeserializeAsyncEnumerable<T>(assetStream);
        
        await foreach (var content in contents) {
            yield return content;
        }
    }
    
    /// <summary>
    /// Downloads a JSON file from a GitHub release and deserializes it into the specified object type
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize the JSON into</typeparam>
    /// <param name="filename">The name of the JSON file to download</param>
    /// <returns>The deserialized object of type T if successful, or null if the file is not found or deserialization fails</returns>
    public async Task<T?> DownloadJsonAsync<T>(string filename) where T : class {
        // Get the GitHub release information
        var release = await GetGitHubReleaseAsync();
        // Find the asset with the specified filename
        var asset = release?.Assets?.FirstOrDefault(n => n.Name.Equals(filename));
        // Download the asset as a stream
        await using var assetStream = await _httpClient.GetStreamAsync(asset?.BrowserDownloadUrl);
        // Deserialize the JSON content into the specified object type
        return await JsonSerializer.DeserializeAsync<T>(assetStream);
    }

    public async Task<GitHubRelease?> GetGitHubReleaseAsync() {
        // Download the asset as a stream.
        await using var response = await _httpClient.GetStreamAsync(_httpClient.BaseAddress);
        // Deserialize the JSON content into the specified object type
        return await JsonSerializer.DeserializeAsync<GitHubRelease>(response);
    }
}
