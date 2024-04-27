using ImeSense.Launchers.Belarus.Core.Models;

namespace ImeSense.Launchers.Belarus.Core.Services;

public interface IGitStorageApiService
{
    Task<T?> DownloadJsonAsync<T>(string filename, Uri? uriRepository = null, CancellationToken cancellationToken = default) where T : class;

    Task<GitHubRelease?> GetLastReleaseAsync(Uri? uriRepository = null, CancellationToken cancellationToken = default);

    Task<IEnumerable<Tag?>?> GetTagsAsync(Uri? uriRepository = null, CancellationToken cancellationToken = default);

    Task<GitHubRelease?> GetReleaseAsync(string tag, Uri? uriRepository = null, CancellationToken cancellationToken = default);
}
