namespace ImeSense.Launchers.Belarus.Core.Storage;

public static class PathStorage {
    public static string LauncherSetting => Path.Combine(DirectoryStorage.User, FileNameStorage.LauncherSetting);
    public static string CurrentRelease => Path.Combine(DirectoryStorage.User, FileNameStorage.CurrentRelease);
    public static string GameUser => Path.Combine(DirectoryStorage.User, FileNameStorage.GameSetting);
}
