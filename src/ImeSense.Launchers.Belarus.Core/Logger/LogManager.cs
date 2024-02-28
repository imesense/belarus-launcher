using Serilog;

namespace ImeSense.Launchers.Belarus.Core.Logger;

public static class LogManager {
    public static ILogger CreateLogger(string pathLog, bool isDeleteOldLog = true) {
        if (isDeleteOldLog && File.Exists(pathLog)) {
            File.Delete(pathLog);
        }

        return new LoggerConfiguration()
            .WriteTo.File(pathLog, rollingInterval: RollingInterval.Infinite)
            .WriteTo.Debug()
            .CreateLogger();
    }
}
