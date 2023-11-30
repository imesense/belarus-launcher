using StalkerBelarus.Launcher.Core.Models;

namespace StalkerBelarus.Launcher.Core.Services;

public interface IGitStorageApiService {
    IAsyncEnumerable<T?> DownloadJsonArrayAsync<T>(string filename) where T : class;
    Task<T?> DownloadJsonAsync<T>(string filename) where T : class;
    Task<GitHubRelease?> GetLastReleaseAsync();
    IAsyncEnumerable<Tag?> GetTagsAsync();
    Task<GitHubRelease?> GetReleaseAsync(string tag);
}
