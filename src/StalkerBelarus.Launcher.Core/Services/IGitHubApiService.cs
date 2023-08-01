using StalkerBelarus.Launcher.Core.Models;

namespace StalkerBelarus.Launcher.Core.Services;

public interface IGitHubApiService {
    Task<T?> DownloadJsonAsync<T>(string filename) where T : class;
    Task<GitHubRelease?> GetGitHubReleaseAsync();
}
