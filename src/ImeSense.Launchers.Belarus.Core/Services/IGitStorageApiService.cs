using ImeSense.Launchers.Belarus.Core.Models;

namespace ImeSense.Launchers.Belarus.Core.Services;

public interface IGitStorageApiService {
    IAsyncEnumerable<T?> DownloadJsonArrayAsync<T>(string filename) where T : class;
    Task<T?> DownloadJsonAsync<T>(string filename) where T : class;
    Task<GitHubRelease?> GetLastReleaseAsync();
    Task<IEnumerable<Tag?>?> GetTagsAsync(Uri? uriRepository = null);
    Task<GitHubRelease?> GetReleaseAsync(string tag);
}
