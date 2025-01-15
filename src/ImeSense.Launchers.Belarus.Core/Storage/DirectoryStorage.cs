namespace ImeSense.Launchers.Belarus.Core.Storage;

public static class DirectoryStorage
{
    public static string Base => Path.GetDirectoryName(Environment.ProcessPath)!;
    public static string CurrentDirectory => Directory.GetCurrentDirectory();
    public static string Binaries => Path.Combine(CurrentDirectory, "binaries");
    public static string Resources => Path.Combine(CurrentDirectory, "resources");
    public static string Patches => Path.Combine(CurrentDirectory, "patches");
    public static string LegacyUserData => Path.Combine(CurrentDirectory, "_user_");
    public static string AppData => Path.Combine(CurrentDirectory, "appdata");
    public static string LaucherData => Path.Combine(AppData, "laucher");
    public static string LauncherLogs => Path.Combine(LaucherData, "logs");
    public static string LauncherCache => Path.Combine(LaucherData, "cache");
}
