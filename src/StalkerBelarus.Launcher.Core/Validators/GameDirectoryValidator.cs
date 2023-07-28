using Microsoft.Extensions.Logging;

namespace StalkerBelarus.Launcher.Core.Validators;

/// <summary>
/// Class for validating game directories
/// </summary>
public class GameDirectoryValidator
{
    private readonly ILogger<GameDirectoryValidator> _logger;

    public GameDirectoryValidator(ILogger<GameDirectoryValidator> logger) {
        _logger = logger;
    }
    
    /// <summary>
    /// Check if the directory contains all the required files
    /// </summary>
    public bool IsDirectoryValid()
    {
        // Check if the "xrEngine.exe" file exists in the "BinariesDirectory" path
        // If it exists, the directory is not valid
        if (File.Exists(Path.Combine(FileLocations.BinariesDirectory, "xrEngine.exe")))
        {
            return false;
        }

        // Check if the number of files in the "ResourcesDirectory" path is greater than or equal to 11
        // If there are at least 11 files, the directory is valid
        return CountFilesInDirectory(FileLocations.ResourcesDirectory) >= 11;
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
}

