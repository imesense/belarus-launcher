using ImeSense.Launchers.Belarus.Core.Models;

namespace ImeSense.Launchers.Belarus.Core.Services;

public interface IGitStorageApiService {
    //IAsyncEnumerable<T?> DownloadJsonArrayAsync<T>(string filename, Uri? uriRepository = null) where T : class;
    Task<T?> DownloadJsonAsync<T>(string filename, Uri? uriRepository = null) where T : class;
    Task<GitHubRelease?> GetLastReleaseAsync(Uri? uriRepository = null);
    Task<IEnumerable<Tag?>?> GetTagsAsync(Uri? uriRepository = null);
    Task<GitHubRelease?> GetReleaseAsync(string tag, Uri? uriRepository = null);
}
