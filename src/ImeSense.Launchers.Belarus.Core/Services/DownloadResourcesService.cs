using System.Collections.Concurrent;
using System.Diagnostics;

using ImeSense.Launchers.Belarus.Core.FileHashVerification;
using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.Core.Models;
using ImeSense.Launchers.Belarus.Core.Storage;

using Microsoft.Extensions.Logging;

namespace ImeSense.Launchers.Belarus.Core.Services;

public class DownloadResourcesService : IDownloadResourcesService
{
    private readonly ILogger<DownloadResourcesService> _logger;
    private readonly IGitStorageApiService _gitStorageApiService;
    private readonly IFileDownloadManager _fileDownloadManager;
    private readonly ILauncherStorage _launcherStorage;
    private readonly HashChecker _hashChecker;

    private IList<GameResource>? _hashResources;

    public DownloadResourcesService(ILogger<DownloadResourcesService> logger,
        IGitStorageApiService gitStorageApiService,
        IFileDownloadManager fileDownloadManager,
        ILauncherStorage launcherStorage, HashChecker hashChecker)
    {
        _logger = logger;
        _gitStorageApiService = gitStorageApiService;
        _fileDownloadManager = fileDownloadManager;
        _launcherStorage = launcherStorage;
        _hashChecker = hashChecker;
    }

    public async Task<IDictionary<string, Uri>?> GetFilesForDownloadAsync(IProgress<int> progress,
        CancellationToken token = default)
    {
        var filesRes = new ConcurrentDictionary<string, Uri>();
        _hashResources ??= await _gitStorageApiService
            .DownloadJsonAsync<IList<GameResource>>(FileNameStorage.HashResources, UriStorage.BelarusApiUri);

        var release = _launcherStorage.GitHubRelease;
        if (release is null) {
            return filesRes;
        }

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        if (_hashResources is null) {
            throw new NullReferenceException("HashResources object is null");
        }

        var totalTasks = _hashResources.Count;
        var completedTasks = 0;

        if (release.Assets is null) {
            throw new NullReferenceException("Assets is null");
        }

        var gameResourceTasks = new List<Task>();
        foreach (var asset in release.Assets) {
            if (asset is null) {
                continue;
            }
            if (asset.BrowserDownloadUrl is null) {
                continue;
            }

            var assetFile = _hashResources.FirstOrDefault(x => x.Title.Equals(asset.Name, StringComparison.OrdinalIgnoreCase));
            if (assetFile is null) {
                continue;
            }

        #if DEBUG
            if (assetFile.Directory.Equals("resources")) {
                continue;
            }
        #endif
            var filePath = Path.Combine(DirectoryStorage.Base, assetFile.Directory, assetFile.Title);

            if (!File.Exists(filePath)) {
                filesRes.TryAdd(filePath, asset.BrowserDownloadUrl);
                CalcProgress(ref completedTasks, progress, totalTasks);
            } else {
            #if DEBUG
                if (assetFile.Directory.Equals("resources")) {
                    continue;
                }
            #endif

                await using var fileStream = File.OpenRead(filePath);
                if (fileStream.Length > 100000000) {
                    gameResourceTasks.Add(Task.Run(async () => {
                        _logger.LogInformation("File: {File}", assetFile.Title);
                        var verifyFile = await _hashChecker.VerifyFileHashAsync(filePath, assetFile.Hash, token);
                        if (!verifyFile) {
                            filesRes.TryAdd(fileStream.Name, asset.BrowserDownloadUrl);
                            _logger.LogWarning("The {FileName} is corrupted", assetFile.Title);
                        }

                        CalcProgress(ref completedTasks, progress, totalTasks);
                    }, token));
                } else {
                    var verifyFile = _hashChecker.VerifyFileHash(fileStream, assetFile.Hash);
                    if (!verifyFile) {
                        filesRes.TryAdd(filePath, asset.BrowserDownloadUrl);
                    }

                    CalcProgress(ref completedTasks, progress, totalTasks);
                }
            }
        }

        await Task.WhenAll(gameResourceTasks);

        stopwatch.Stop();
        _logger.LogInformation("Hash calculation time: {Time}", stopwatch.ElapsedMilliseconds);
        progress.Report(0);

        return filesRes;
    }

    private void CalcProgress(ref int completedTasks, IProgress<int> progress, int totalTasks)
    {
        Interlocked.Increment(ref completedTasks);
        var progressPercentage = (int) ((float) completedTasks / totalTasks * 100);
        progress.Report(progressPercentage);
        _logger.LogInformation("Progress: {Num}%", progressPercentage);
    }

    public async Task DownloadAsync(string path, Uri url, IProgress<int> progress, CancellationToken token = default)
    {
        try {
            var dirInfo = new DirectoryInfo(Path.GetDirectoryName(path)!);
            if (!dirInfo.Exists) {
                dirInfo.Create();
            }
            _hashResources ??= await _gitStorageApiService
                .DownloadJsonAsync<IList<GameResource>>(FileNameStorage.HashResources, UriStorage.BelarusApiUri);
            var verifyFile = false;
            do {
                try {
                    await _fileDownloadManager.DownloadAsync(url, path, progress, token);
                    // Check the downloaded file for integrity
                    var assetName = Path.GetFileName(path);
                    var gameResource = _hashResources?.FirstOrDefault(x => x.Title.Equals(assetName, StringComparison.OrdinalIgnoreCase));
                    verifyFile = await _hashChecker.VerifyFileHashAsync(path, gameResource!.Hash, token);
                    if (!verifyFile) {
                        File.Delete(path);
                    }
                } catch (HttpRequestException ex) when (ex.Message.Contains("416")) {
                    _logger.LogInformation("Unsuccessful attempt to download the file! The file will be deleted and downloaded again");
                    File.Delete(path);
                }
            } while (!verifyFile);

            progress.Report(0);
        } catch (OperationCanceledException ex) {
            _logger.LogInformation("{Message}", ex.Message);
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
