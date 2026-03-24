using System.IO.Compression;

using Belarus.Launcher.Core.Manager;
using Belarus.Launcher.Core.Storage;

using Microsoft.Extensions.Logging;

namespace Belarus.Launcher.Core.Services;

public class UpdaterService(ILogger<UpdaterService> logger, IGitStorageApiService gitStorageApiService, IFileDownloadManager fileDownloadManager) : IUpdaterService
{
    private readonly ILogger<UpdaterService> _logger = logger;
    private readonly IGitStorageApiService _gitStorageApiService = gitStorageApiService;
    private readonly IFileDownloadManager _fileDownloadManager = fileDownloadManager;

    public async Task UpdaterAsync(Uri uri, string fileSavePath, CancellationToken cancellationToken = default)
    {
        CleanUpTempFiles(); // Delete files if user canceled download

        var fileName = Path.GetFileNameWithoutExtension(fileSavePath);
        var fullFileName = Path.GetFileName(fileSavePath);

        var lastRelease = await _gitStorageApiService.GetLastReleaseAsync(uri, cancellationToken)
            ?? throw new NullReferenceException("Latest release is null!");
        var sblauncher = lastRelease.Assets?.FirstOrDefault(x => x.Name.Equals(fullFileName))
            ?? throw new NullReferenceException($"{fileName} asset is null!");

        if (sblauncher.BrowserDownloadUrl == null)
        {
            throw new NullReferenceException("Browser download url is null!");
        }
        var progress = new Progress<int>(percentage =>
        {
            _logger.LogInformation("{fileName} is {percentage}% downloaded", fileName, percentage);
        });

        var pathDownloadFolder = Path.Combine(DirectoryStorage.CurrentDirectory, "temp");
        var fileDownloadPath = Path.Combine(pathDownloadFolder, fullFileName);

        if (!Directory.Exists(pathDownloadFolder))
        {
            Directory.CreateDirectory(pathDownloadFolder);
        }

        await _fileDownloadManager.DownloadAsync(sblauncher.BrowserDownloadUrl, fileDownloadPath, progress, cancellationToken);

        ExtractFile(fileDownloadPath, fileSavePath);
        CleanUpTempFiles();
    }

    private static void ExtractFile(string sourcePath, string destinationPath)
    {
        if (Path.GetExtension(sourcePath).Equals(".zip", StringComparison.OrdinalIgnoreCase))
        {
            ZipFile.ExtractToDirectory(sourcePath, DirectoryStorage.CurrentDirectory, true);
        }
        else
        {
            if (File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
            }
            File.Move(sourcePath, destinationPath);
        }
    }

    private static void CleanUpTempFiles()
    {
        var pathDownloadFolder = Path.Combine(DirectoryStorage.CurrentDirectory, "temp");
        if (Directory.Exists(pathDownloadFolder))
        {
            Directory.Delete(pathDownloadFolder, true);
        }
    }
}
