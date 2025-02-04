using Avalonia;
using Avalonia.ReactiveUI;

using ImeSense.Launchers.Belarus.Core.Logger;
using ImeSense.Launchers.Belarus.Core.Storage;

using Microsoft.Extensions.Logging;

using Serilog;

namespace ImeSense.Launchers.Belarus.Desktop;

internal class Program
{
    private const string _mutexName = "Belarus.Launcher.Desktop";
    private static Mutex? _mutex;
    private static Microsoft.Extensions.Logging.ILogger? _logger;

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var pathLog = Path.Combine(DirectoryStorage.LauncherLogs, FileNameStorage.LauncherLog);
        var factory = LoggerFactory.Create(builder => builder.AddSerilog(LogManager.CreateLoggerConsole(pathLog)));
        _logger = factory.CreateLogger<Program>();

        var isMutexCreated = false;
        try
        {
            _mutex = new Mutex(initiallyOwned: false, _mutexName, out isMutexCreated);
        }
        catch (Exception exception)
        {
            _logger.LogError("{Message} \n {StackTrace}", exception.Message, exception.StackTrace);
            Log.CloseAndFlush();
            _mutex?.Dispose();
            throw;
        }
        if (!isMutexCreated)
        {
            return;
        }

        try
        {
#if DEBUG
            StartApp(args);
#else
            try
            {
                StartApp(args);
            }
            catch (Exception exception)
            {
                _logger.LogError("{Message} \n {StackTrace}", exception.Message, exception.StackTrace);
                Log.CloseAndFlush();
                _mutex?.Dispose();
                throw;
            }
#endif
        }
        finally
        {
            Log.CloseAndFlush();
            _mutex?.Dispose();
        }
    }

    private static void StartApp(string[] args)
    {
        _logger?.LogInformation("{Info}", InformationPrinter.GetStartupInfo("Belarus Launcher"));
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace()
                .UseReactiveUI();
    }
}
