using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace ImeSense.Launchers.Belarus.Core.Logger;

public static class LogManager
{
    private static readonly string _outputTemplate = "{Timestamp:HH:mm:ss} [{Level:u3}] [{ThreadName}:{ThreadId}] {Message:lj}{NewLine}{Exception}";

    public static ILogger CreateLogger(string pathLog)
    {
        return BaseLoggerConfiguration(pathLog)
            .CreateLogger();
    }

    public static ILogger CreateLoggerConsole(string pathLog)
    {
        return BaseLoggerConfiguration(pathLog)
            .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: _outputTemplate)
            .CreateLogger();
    }

    private static LoggerConfiguration BaseLoggerConfiguration(string pathLog)
    {
        return new LoggerConfiguration()
            .Enrich.WithThreadId()
            .Enrich.WithThreadName()
            .Enrich.WithProperty(name: "ThreadName", value: "MainThread")
            .WriteTo.Debug(outputTemplate: _outputTemplate)
            .WriteTo.File($"{pathLog}.log", outputTemplate: _outputTemplate, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 5);
    }
}
