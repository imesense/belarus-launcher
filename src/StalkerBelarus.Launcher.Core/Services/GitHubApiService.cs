using System.Text.Json;

using Microsoft.Extensions.Logging;

using StalkerBelarus.Launcher.Core.Models;
using System.Net.Http.Headers;

namespace StalkerBelarus.Launcher.Core.Services;

public class GitHubApiService : IGitHubApiService {
    private readonly ILogger<GitHubApiService> _logger;
    private readonly HttpClient _httpClient;

    public GitHubApiService(ILogger<GitHubApiService> logger, HttpClient httpClient) {
        _logger = logger;
        _httpClient = httpClient;
    }


    public async Task<GitHubRelease?> GetGitHubReleaseAsync() {
        await using var response = await _httpClient.GetStreamAsync(_httpClient.BaseAddress);
        return await JsonSerializer.DeserializeAsync<GitHubRelease>(response);
    }

}
