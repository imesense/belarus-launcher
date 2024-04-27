namespace ImeSense.Launchers.Belarus.Core.Storage;

public static class PathStorage
{
    public static string LauncherSetting => Path.Combine(DirectoryStorage.AppData, FileNameStorage.LauncherSetting);
    public static string CurrentRelease => Path.Combine(DirectoryStorage.LauncherCache, FileNameStorage.CurrentRelease);
    public static string GameUser => Path.Combine(DirectoryStorage.AppData, FileNameStorage.GameSetting);
}
