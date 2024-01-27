using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.Core.Storage;

namespace ImeSense.Launchers.Belarus.Core.Services;

public class UpdaterService : IUpdaterService {
    private readonly IGitStorageApiService _gitStorageApiService;
    private readonly IFileDownloadManager _fileDownloadManager;

    public UpdaterService(IGitStorageApiService gitStorageApiService, IFileDownloadManager fileDownloadManager) {
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
            Console.WriteLine($"{appName} is {percentage}% downloaded");
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
