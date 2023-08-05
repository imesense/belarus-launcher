using System.Collections.Concurrent;
using System.Diagnostics;

using Microsoft.Extensions.Logging;

using StalkerBelarus.Launcher.Core.FileHashVerification;
using StalkerBelarus.Launcher.Core.Manager;
using StalkerBelarus.Launcher.Core.Models;

namespace StalkerBelarus.Launcher.Core.Services;

public class DownloadResourcesService : IDownloadResourcesService {
    private readonly ILogger<DownloadResourcesService> _logger;
    private readonly IGitHubApiService _gitHubApiService;
    private readonly IFileDownloadManager _fileDownloadManager;
    private readonly HashChecker _hashChecker;

    public DownloadResourcesService(ILogger<DownloadResourcesService> logger, IGitHubApiService gitHubApiService,
        IFileDownloadManager fileDownloadManager, HashChecker hashChecker) {
        _logger = logger;
        _gitHubApiService = gitHubApiService;
        _fileDownloadManager = fileDownloadManager;
        _hashChecker = hashChecker;
    }

    public async Task DownloadsAsync(Progress<int> progress, CancellationTokenSource? tokenSource) {
        if (tokenSource != null) {
            return;
        }

        var hashResources = await _gitHubApiService
            .DownloadJsonAsync<IList<GameResource>>(FileNamesStorage.HashResources);
        if (hashResources == null) {
            return;
        }

        var release = await _gitHubApiService.GetGitHubReleaseAsync();
        if (release == null) {
            return;
        }

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        
        var filesRes = new ConcurrentDictionary<string, string>();
        var parallelOptions = new ParallelOptions() {
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        await Parallel.ForEachAsync(release.Assets!, parallelOptions, async (asset, cancellationToken) => {
            var assetFile = hashResources.FirstOrDefault(x => x.Title.Equals(asset.Name.ToLower()));
            if (assetFile == null) {
                return;
            }

            var path = Path.Combine(FileLocations.BaseDirectory, assetFile.Directory, assetFile.Title);
            if (!File.Exists(path)) {
                filesRes.TryAdd(path, asset.BrowserDownloadUrl);
                return;
            }

            var verifyFile = await _hashChecker.VerifyFileHashAsync(path, assetFile.Hash, cancellationToken);
            if (!verifyFile) {
                filesRes.TryAdd(path, asset.BrowserDownloadUrl);
                _logger.LogWarning("The {FileName} is corrupted", assetFile.Title);
            }
        });
        stopwatch.Stop();
        _logger.LogInformation("Hash calculation time: {Time}", stopwatch.ElapsedMilliseconds);

        foreach (var file in filesRes) {
            using (tokenSource = new CancellationTokenSource()) {
                try {
                    var dirInfo = new DirectoryInfo(Path.GetDirectoryName(file.Key)!);
                    if (!dirInfo.Exists) {
                        dirInfo.Create();
                    }
                    
                    await _fileDownloadManager.DownloadAsync(file.Value, file.Key, progress, tokenSource.Token);
                } catch (OperationCanceledException) {
                } catch (HttpRequestException ex) {
                    // 416 (Requested Range Not Satisfiable)
                    if (ex.Message.Contains("416")) {
                        _logger.LogInformation("The file has already been uploaded");
                    } else {
                        _logger.LogError("HttpRequestException - {Message}", ex.Message);
                    }
                } catch (Exception exception) {
                    _logger.LogError("{Message}", exception.Message);
                }
            }
        }
    }
}
