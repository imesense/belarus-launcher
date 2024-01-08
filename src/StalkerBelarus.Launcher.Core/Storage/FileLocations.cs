namespace StalkerBelarus.Launcher.Core.Storage;

public static class FileLocations {
    public static string BaseDirectory => Path.GetDirectoryName(Environment.ProcessPath)!;
    public static string BinariesDirectory => Path.Combine(BaseDirectory, "binaries");
    public static string ResourcesDirectory => Path.Combine(BaseDirectory, "resources");
    public static string PatchesDirectory => Path.Combine(BaseDirectory, "patches");
    public static string UserDirectory => Path.Combine(BaseDirectory, "_user_");
    public static string LogsDirectory => Path.Combine(UserDirectory, "logs");
    public static string UserSettingPath => Path.Combine(UserDirectory, "sblauncher.json");
    public static string CurrentRelease => Path.Combine(UserDirectory, "CurrentRelease.json");
}
