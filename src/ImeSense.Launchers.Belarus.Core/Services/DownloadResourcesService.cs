using System.Collections.Concurrent;
using System.Diagnostics;

using Microsoft.Extensions.Logging;

using ImeSense.Launchers.Belarus.Core.FileHashVerification;
using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.Core.Models;
using ImeSense.Launchers.Belarus.Core.Storage;

namespace ImeSense.Launchers.Belarus.Core.Services;

public class DownloadResourcesService : IDownloadResourcesService {
    private readonly ILogger<DownloadResourcesService> _logger;
    private readonly IGitStorageApiService _gitStorageApiService;
    private readonly IFileDownloadManager _fileDownloadManager;
    private readonly ILauncherStorage _launcherStorage;
    private readonly HashChecker _hashChecker;

    private IList<GameResource>? _hashResources;
    
    public DownloadResourcesService(ILogger<DownloadResourcesService> logger,
        IGitStorageApiService gitStorageApiService,
        IFileDownloadManager fileDownloadManager,
        ILauncherStorage launcherStorage, HashChecker hashChecker) {
        _logger = logger;
        _gitStorageApiService = gitStorageApiService;
        _fileDownloadManager = fileDownloadManager;
        _launcherStorage = launcherStorage;
        _hashChecker = hashChecker;
    }

    public async Task<IDictionary<string, Uri>?> GetFilesForDownloadAsync(IProgress<int> progress) {
        var filesRes = new ConcurrentDictionary<string, Uri>();
        _hashResources ??= await _gitStorageApiService
            .DownloadJsonAsync<IList<GameResource>>(FileNamesStorage.HashResources, UriStorage.BelarusApiUri);

        var release = _launcherStorage.GitHubRelease;
        if (release is null) {
            return filesRes;
        }

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var parallelOptions = new ParallelOptions() {
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        if (_hashResources is null) {
            throw new NullReferenceException("HashResources object is null");
        }

        var totalTasks = _hashResources.Count;
        var completedTasks = 0;
        await Parallel.ForEachAsync(release.Assets!, parallelOptions, async (asset, cancellationToken) => {
            if (asset is null) {
                return;
            }
            if (asset.BrowserDownloadUrl is null) {
                return;
            }

            var assetFile = _hashResources.FirstOrDefault(x => x.Title.Equals(asset.Name, StringComparison.OrdinalIgnoreCase));
            if (assetFile is null) {
                return;
            }

            var path = Path.Combine(FileLocations.BaseDirectory, assetFile.Directory, assetFile.Title);
            if (!File.Exists(path)) {
                var result = filesRes.TryAdd(path, asset.BrowserDownloadUrl);
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
        progress.Report(0);

        return filesRes;
    }

    public async Task DownloadAsync(string path, Uri url, IProgress<int> progress,
        CancellationTokenSource? tokenSource) {
        using (tokenSource = new CancellationTokenSource()) {
            try {
                var dirInfo = new DirectoryInfo(Path.GetDirectoryName(path)!);
                if (!dirInfo.Exists) {
                    dirInfo.Create();
                }

                _hashResources ??= await _gitStorageApiService
                    .DownloadJsonAsync<IList<GameResource>>(FileNamesStorage.HashResources, UriStorage.BelarusApiUri);

                bool verifyFile;
                do {
                    await _fileDownloadManager.DownloadAsync(url, path, progress, tokenSource.Token);
                    // Check the downloaded file for integrity
                    var assetName = Path.GetFileName(path);
                    var gameResource = _hashResources?.FirstOrDefault(x => x.Title.Equals(assetName, 
                        StringComparison.OrdinalIgnoreCase));
                    verifyFile = await _hashChecker.VerifyFileHashAsync(path, gameResource!.Hash);
                    if (!verifyFile) {
                        File.Delete(path);
                    }
                } while (!verifyFile);
                
                progress.Report(0);
            } catch (OperationCanceledException) {
            } catch (HttpRequestException ex) {
                // 416 (Requested Range Not Satisfiable)
                if (ex.Message.Contains("416")) {
                    _logger.LogInformation("The file has already been uploaded");
                } else {
                    _logger.LogError("HttpRequestException - {Message}", ex.Message);
                    throw;
                }
            } catch (Exception exception) {
                _logger.LogError("{Message}", exception.Message);
            }
        }
    }
}
