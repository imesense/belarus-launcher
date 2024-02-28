using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace ImeSense.Launchers.Belarus.Core.Logger;

public static class LogManager {
    public static ILogger CreateLogger(string pathLog, bool isDeleteOldLog = true) {
        if (isDeleteOldLog && File.Exists(pathLog)) {
            File.Delete(pathLog);
        }

        return new LoggerConfiguration()
            .WriteTo.Debug()
            .WriteTo.File(pathLog, rollingInterval: RollingInterval.Infinite)
            .CreateLogger();
    }

    public static ILogger CreateLoggerConsole(string pathLog, bool isDeleteOldLog = true) {
        if (isDeleteOldLog && File.Exists(pathLog)) {
            File.Delete(pathLog);
        }

        return new LoggerConfiguration()
            .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(pathLog, rollingInterval: RollingInterval.Infinite)
            .WriteTo.Debug()
            .CreateLogger();
    }
}
