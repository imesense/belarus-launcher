using ImeSense.Launchers.Belarus.Core.Models;
using ImeSense.Launchers.Belarus.Core.Storage;

using Microsoft.Extensions.Logging;

namespace ImeSense.Launchers.Belarus.Core.Validators;

/// <summary>
/// Class for validating game directories
/// </summary>
public class GameDirectoryValidator(ILogger<GameDirectoryValidator> logger, ILauncherStorage launcherStorage)
{
    private readonly ILogger<GameDirectoryValidator> _logger = logger;
    private readonly ILauncherStorage _launcherStorage = launcherStorage;

    /// <summary>
    /// Check if the directory contains all the required files
    /// </summary>
    public bool IsDirectoryValid()
    {
        // Check if the Binaries directory exists
        if (!Directory.Exists(DirectoryStorage.Binaries))
        {
            return false;
        }

        // Check if the "xrEngine.exe" file exists in the "BinariesDirectory" path
        // If it exists, the directory is not valid
        if (!File.Exists(Path.Combine(DirectoryStorage.Binaries, "xrEngine.exe")))
        {
            return false;
        }

        // Check if the Resources directory exists
        if (!Directory.Exists(DirectoryStorage.Resources))
        {
            return false;
        }

        if (_launcherStorage.IsCheckGitHubConnection)
        {
            //Check the size of folders and files
            if (!CompareFileSizes())
            {
                return false;
            }
        }

        // Check if the number of files in the "ResourcesDirectory" path is greater than or equal to 11
        // If there are at least 11 files, the directory is valid
        return CountFilesInDirectory(DirectoryStorage.Resources) >= 11;
    }

    /// <summary>
    /// Method to count the number of files in a given directory path
    /// </summary>
    /// <param name="path">Directory path</param>
    /// <returns>Count files</returns>
    private int CountFilesInDirectory(string path)
    {
        try
        {
            // Get the list of files in the directory
            var files = Directory.GetFiles(path);

            // Return the count of files in the directory
            return files.Length;
        }
        catch (Exception ex)
        {
            // If an error occurs while accessing the directory, output the error message
            _logger.LogError("{Message}", ex.Message);
            return 0;
        }
    }

    /// <summary>
    /// Compares sizes of server files with local files and logs result
    /// </summary>
    /// <returns>True if server files are larger, false if local files are larger, and null if sizes are equal</returns>
    private bool CompareFileSizes()
    {
        if (_launcherStorage.GitHubRelease is null)
        {
            _logger.LogError("Server files are larger than local files");
            return false;
        }

        if (_launcherStorage.GitHubRelease.Assets is null || _launcherStorage.GitHubRelease.Assets.Count == 0)
        {
            _logger.LogError("Array with assets does not exist or is empty!");
            return false;
        }

        // Calculate sizes of server files and local files
        var filteredAssets = _launcherStorage.GitHubRelease.Assets
            .Where(asset => !IgnoreFileStorage.IgnoreFiles
            .Any(ignoreFile => ignoreFile.Equals(asset.Name, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        var serverSize = CalculateServerFilesSize(filteredAssets);
        _logger.LogInformation("Server files size: {Size}", serverSize);
        var localSize = CalculateLocalFilesSize(filteredAssets.Select(x => x.Name));
        _logger.LogInformation("Local files size: {Size}", localSize);

        // Compare sizes and log the result
        if (serverSize > localSize)
        {
            _logger.LogInformation("Server files are larger than local files");
            return false;
        }
        else if (serverSize < localSize)
        {
            _logger.LogInformation("Local files are larger than server files");
            return false;
        }
        else
        {
            _logger.LogInformation("Sizes of local and server files are equal");
            return true;
        }
    }

    /// <summary>
    /// Calculates total size of files associated with GitHub release
    /// </summary>
    /// <param name="assets">Collection of assets associated with GitHub release</param>
    /// <returns>Total size of files in GitHub release</returns>
    private long CalculateServerFilesSize(IEnumerable<Asset> assets)
    {
        if (assets is null || !assets.Any())
        {
            _logger.LogError("Array with assets does not exist or is empty!");
            return 0;
        }

        return assets.Sum(asset => asset.Size);
    }

    /// <summary>
    /// Calculates total size of local files in specific directories and logs file count in each directory
    /// </summary>
    /// <param name="validFilesName">Collection of valid file names</param>
    /// <returns>Total size of local files</returns>
    private long CalculateLocalFilesSize(IEnumerable<string> validFilesName)
    {
        long totalSize = 0;

        // Directories containing local files
        var directories = new List<string> {
            DirectoryStorage.Binaries,
            DirectoryStorage.Resources,
            DirectoryStorage.Patches
        };

        foreach (var directoryPath in directories)
        {
            if (!Directory.Exists(directoryPath))
            {
                _logger.LogWarning("Directory {Directory} does not exist", directoryPath);
                continue;
            }

            var directory = new DirectoryInfo(directoryPath);
            var files = directory.GetFiles();

            _logger.LogInformation("Directory {Directory} contains {Count} files", directoryPath, files.Length);

            // Filter out ignored files and sum their sizes
            var validFiles = files
                .Where(file => validFilesName
                .Any(ignoreFile => ignoreFile.Equals(file.Name, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            _logger.LogInformation("Current: validFiles {Count}", validFiles.Count);

            totalSize += validFiles.Sum(file => file.Length);
        }

        return totalSize;
    }
}
