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

        var hashResources = await _gitHubApiService.DownloadJsonAsync<GameResources>(FileNamesStorage.HashResources);
        if (hashResources == null) {
            return;
        }
        
        var release = await _gitHubApiService.GetGitHubReleaseAsync();
        if (release == null) {
            return;
        }
        
        foreach (var asset in release.Assets!) {
            var fileName = asset.Name.ToLower();
            var filePath = FileLocations.BaseDirectory;
            var hashFile = "";
            
            var assetBinFile = hashResources.Binaries?.FirstOrDefault(x=> x.Title.Equals(fileName.ToLower()));
            var assetResFile = hashResources.Resources?.FirstOrDefault(x=> x.Title.Equals(fileName.ToLower()));
            var assetPatchFile = hashResources.Patches?.FirstOrDefault(x=> x.Title.Equals(fileName.ToLower()));
            if (assetBinFile != null) {
                filePath = Path.Combine(FileLocations.BinariesDirectory, fileName);
                hashFile = assetBinFile.Hash;
            } else if (assetResFile != null) {
                filePath = Path.Combine(FileLocations.ResourcesDirectory, fileName);
                hashFile = assetResFile.Hash;
            } else if (assetPatchFile != null) {
                filePath = Path.Combine(FileLocations.PatchesDirectory, fileName);
                hashFile = assetPatchFile.Hash;
            }
            
            if (File.Exists(filePath)) {
                var verifyFile = await _hashChecker.VerifyFileHashAsync(filePath, hashFile);
                if (verifyFile) {
                    continue;
                }

                _logger.LogWarning("The {FileName} is corrupted", fileName);
            }
            
            using (tokenSource = new CancellationTokenSource()) {
                try {
                    var dirInfo = new DirectoryInfo(Path.GetDirectoryName(filePath)!);
                    if (!dirInfo.Exists) {
                        dirInfo.Create();
                    }
                    
                    await _fileDownloadManager.DownloadAsync(asset.BrowserDownloadUrl, filePath, progress,
                        tokenSource.Token);
                } catch (OperationCanceledException) {
                }
                catch (HttpRequestException ex) {
                    // 416 (Requested Range Not Satisfiable)
                    if (ex.Message.Contains("416")) {
                        _logger.LogInformation("The file has already been uploaded");
                    } else {
                        _logger.LogError("HttpRequestException - {Message}", ex.Message);
                    }
                }
                catch (Exception exception) {
                    _logger.LogError("{Message}", exception.Message);
                }
            } 
        }
    }
}
