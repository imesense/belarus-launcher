using ImeSense.Launchers.Belarus.Core.Models;

namespace ImeSense.Launchers.Belarus.Core.Services;

public interface IGitStorageApiService {
    IAsyncEnumerable<T?> DownloadJsonArrayAsync<T>(string filename) where T : class;
    Task<T?> DownloadJsonAsync<T>(string filename) where T : class;
    Task<GitHubRelease?> GetLastReleaseAsync();
    Task<IEnumerable<Tag?>?> GetTagsAsync();
    Task<GitHubRelease?> GetReleaseAsync(string tag);
}
