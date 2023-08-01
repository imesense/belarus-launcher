using StalkerBelarus.Launcher.Core.Models;

namespace StalkerBelarus.Launcher.Core.Services;

public interface IGitHubApiService {
    Task<GitHubRelease?> GetGitHubReleaseAsync();
}
