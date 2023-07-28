namespace StalkerBelarus.Launcher.Core;

public static class FileLocations {
    public static string BaseDirectory => AppDomain.CurrentDomain.BaseDirectory;
    public static string UserDirectory => Path.Combine(BaseDirectory, "_user_");
    public static string LogsDirectory => Path.Combine(UserDirectory, "_logs_");
    public static string UserSettingPath => Path.Combine(UserDirectory, "sblauncher.json");
}
