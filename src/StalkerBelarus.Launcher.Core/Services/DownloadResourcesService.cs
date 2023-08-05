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

    public async Task<IDictionary<string, string>?> GetFilesForDownloadAsync(IProgress<int> progress) {
        var filesRes = new ConcurrentDictionary<string, string>();
        
        var hashResources = await _gitHubApiService
            .DownloadJsonAsync<IList<GameResource>>(FileNamesStorage.HashResources);
        if (hashResources == null) {
            return filesRes;
        }

        var release = await _gitHubApiService.GetGitHubReleaseAsync();
        if (release == null) {
            return filesRes;
        }

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        
        var parallelOptions = new ParallelOptions() {
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };
        var totalTasks = hashResources.Count;
        var completedTasks = 0;
        await Parallel.ForEachAsync(release.Assets!, parallelOptions, async (asset, cancellationToken) => {
            var assetFile = hashResources.FirstOrDefault(x => x.Title.Equals(asset.Name.ToLower()));
            if (assetFile == null) {
                return;
            }

            var path = Path.Combine(FileLocations.BaseDirectory, assetFile.Directory, assetFile.Title);
            if (!File.Exists(path)) {
                filesRes.TryAdd(path, asset.BrowserDownloadUrl);
            } else {
                var verifyFile = await _hashChecker.VerifyFileHashAsync(path, assetFile.Hash, cancellationToken);
                if (!verifyFile) {
                    filesRes.TryAdd(path, asset.BrowserDownloadUrl);
                    _logger.LogWarning("The {FileName} is corrupted", assetFile.Title);
                }
            }
            Interlocked.Increment(ref completedTasks);
            var progressPercentage = (int)((float)completedTasks / totalTasks * 100);
            progress.Report(progressPercentage);
            _logger.LogInformation("Progress: {Num}%", progressPercentage);
        });
        stopwatch.Stop();
        _logger.LogInformation("Hash calculation time: {Time}", stopwatch.ElapsedMilliseconds);

        return filesRes;
    }
    
    public async Task DownloadAsync(string path, string url, IProgress<int> progress, 
        CancellationTokenSource? tokenSource) {
        using (tokenSource = new CancellationTokenSource()) {
            try {
                var dirInfo = new DirectoryInfo(Path.GetDirectoryName(path)!);
                if (!dirInfo.Exists) {
                    dirInfo.Create();
                }

                await _fileDownloadManager.DownloadAsync(url, path, progress, tokenSource.Token);
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
