namespace Belarus.Launcher.Core.Storage;

public static class PathStorage
{
    // Актуально для версии 1.0 / 1.1
    public static string LegacyLauncherSettings => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), FileNameStorage.LauncherSetting);
    // Актуально для версии 2.0 / 2.1
    public static string V2LauncherSetting => Path.Combine(DirectoryStorage.LegacyUserData, FileNameStorage.LauncherSetting);
    // Актуально для версии 3.0 и новее
    public static string LauncherSetting => Path.Combine(DirectoryStorage.LaucherData, FileNameStorage.LauncherSetting);
    public static string CurrentRelease => Path.Combine(DirectoryStorage.LauncherCache, FileNameStorage.CurrentRelease);
    public static string LegacyGameUser => Path.Combine(DirectoryStorage.LegacyUserData, FileNameStorage.GameSetting);
    public static string GameUser => Path.Combine(DirectoryStorage.AppData, FileNameStorage.GameSetting);
    public static string NewsCache => Path.Combine(DirectoryStorage.LauncherCache, FileNameStorage.NewsContent);
    public static string WebResourcesCache => Path.Combine(DirectoryStorage.LauncherCache, FileNameStorage.WebResources);
}
