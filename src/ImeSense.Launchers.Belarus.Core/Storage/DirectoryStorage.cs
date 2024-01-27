namespace ImeSense.Launchers.Belarus.Core.Storage;

public static class DirectoryStorage {
    public static string Base => Path.GetDirectoryName(Environment.ProcessPath)!;
    public static string Binaries => Path.Combine(Base, "binaries");
    public static string Resources => Path.Combine(Base, "resources");
    public static string Patches => Path.Combine(Base, "patches");
    public static string User => Path.Combine(Base, "_user_");
    public static string UserLogs => Path.Combine(User, "logs");
}
