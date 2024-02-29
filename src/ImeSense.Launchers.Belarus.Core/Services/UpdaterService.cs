using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.Core.Storage;

using Microsoft.Extensions.Logging;

namespace ImeSense.Launchers.Belarus.Core.Services;

public class UpdaterService : IUpdaterService {
    private readonly ILogger<UpdaterService> _logger;
    private readonly IGitStorageApiService _gitStorageApiService;
    private readonly IFileDownloadManager _fileDownloadManager;

    public UpdaterService(ILogger<UpdaterService> logger, IGitStorageApiService gitStorageApiService, IFileDownloadManager fileDownloadManager) {
        _logger = logger;
        _gitStorageApiService = gitStorageApiService;
        _fileDownloadManager = fileDownloadManager;
    }

    public async Task UpdaterAsync(Uri uri, string fileSavePath) {
        var appName = Path.GetFileNameWithoutExtension(fileSavePath);
        var fullAppName = Path.GetFileName(fileSavePath);

        var lastRelease = await _gitStorageApiService.GetLastReleaseAsync(uri)
            ?? throw new NullReferenceException("Latest release is null!");
        var sblauncher = lastRelease.Assets?.FirstOrDefault(x => x.Name.Equals(fullAppName))
            ?? throw new NullReferenceException($"{appName} asset is null!");

        if (sblauncher.BrowserDownloadUrl == null) {
            throw new NullReferenceException("Browser download url is null!");
        }
        var progress = new Progress<int>(percentage => {
            _logger.LogInformation("{appName} is {percentage}% downloaded", appName, percentage);
        });

        var pathDownloadFolder = Path.Combine(DirectoryStorage.Base, "temp");
        var fileDownloadPath = Path.Combine(pathDownloadFolder, fullAppName);

        if (!Directory.Exists(pathDownloadFolder)) {
            Directory.CreateDirectory(pathDownloadFolder);
        }

        await _fileDownloadManager.DownloadAsync(sblauncher.BrowserDownloadUrl, fileDownloadPath, progress);

        if (File.Exists(fileSavePath)) {
            File.Delete(fileSavePath);
        }
        File.Move(fileDownloadPath, fileSavePath);

        if (Directory.Exists(pathDownloadFolder)) {
            Directory.Delete(pathDownloadFolder);
        }
    }
}
