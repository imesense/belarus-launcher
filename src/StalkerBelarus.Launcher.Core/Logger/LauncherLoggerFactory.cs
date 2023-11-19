using Serilog;

namespace StalkerBelarus.Launcher.Core.Logger;

public static class LauncherLoggerFactory {
    public static void CreateLogger()
    {
        var pathLog = Path.Combine(FileLocations.LogsDirectory, FileNamesStorage.Log);
        if (File.Exists(pathLog)) {
            File.Delete(pathLog);
        }
        
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(pathLog, rollingInterval: RollingInterval.Infinite)
            .WriteTo.Debug()
            .CreateLogger();
    }
}
