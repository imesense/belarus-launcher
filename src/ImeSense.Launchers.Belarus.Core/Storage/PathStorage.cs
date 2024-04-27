namespace ImeSense.Launchers.Belarus.Core.Storage;

public static class PathStorage
{
    public static string LauncherSetting => Path.Combine(DirectoryStorage.AppData, FileNameStorage.LauncherSetting);
    public static string CurrentRelease => Path.Combine(DirectoryStorage.LauncherCache, FileNameStorage.CurrentRelease);
    public static string GameUser => Path.Combine(DirectoryStorage.AppData, FileNameStorage.GameSetting);
    public static string NewsCache => Path.Combine(DirectoryStorage.LauncherCache, FileNameStorage.NewsContent);
    public static string WebResourcesCache => Path.Combine(DirectoryStorage.LauncherCache, FileNameStorage.WebResources);
}
