using System.Collections.ObjectModel;

namespace ImeSense.Launchers.Belarus.Core.Storage;

/// <summary>
/// Provides a storage for files that should be ignored or excluded from processing in certain parts of the code
/// </summary>
public static class IgnoreFileStorage
{
    /// <summary>
    /// Gets the collection of files to be ignored
    /// </summary>
    public static IReadOnlyCollection<string> IgnoreFiles { get; }

    static IgnoreFileStorage()
    {
        var ignoreFilesArray = new string[]
        {
            FileNameStorage.HashResources,
            FileNameStorage.WebResources,
            FileNameStorage.LegacyNews,
            FileNameStorage.NewsContentRus,
            FileNameStorage.NewsContentEng,
            FileNameStorage.LegacyHash,
        };

        IgnoreFiles = new ReadOnlyCollection<string>(ignoreFilesArray);
    }
}
