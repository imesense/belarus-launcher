namespace ImeSense.Launchers.Belarus.Core.Storage;

public static class DirectoryStorage
{
    public static string Base => Path.GetDirectoryName(Environment.ProcessPath)!;
    public static string Binaries => Path.Combine(Base, "binaries");
    public static string Resources => Path.Combine(Base, "resources");
    public static string Patches => Path.Combine(Base, "patches");
    public static string LegacyUserData => Path.Combine(Base, "_user_");
    public static string AppData => Path.Combine(Base, "appdata");
    public static string LaucherData => Path.Combine(AppData, "laucher");
    public static string LauncherLogs => Path.Combine(LaucherData, "logs");
    public static string LauncherCache => Path.Combine(LaucherData, "cache");
}
