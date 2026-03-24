using Avalonia;
using ReactiveUI.Avalonia;

using Belarus.Launcher.Core.Exceptions;
using Belarus.Launcher.Core.Logger;
using Belarus.Launcher.Core.Storage;

using Microsoft.Extensions.Logging;

using Serilog;

namespace Belarus.Launcher.Desktop;

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
        var factory = LoggerFactory.Create(builder => builder.AddSerilog(LogManager.CreateLogger(pathLog)));
        _logger = factory.CreateLogger<Program>();

        GlobalExceptionHandler.Initialize(_logger);
        _logger.LogInformation("{Info}", InformationPrinter.GetStartupInfo("Belarus Launcher"));

        _mutex = new Mutex(initiallyOwned: false, _mutexName, out bool isMutexCreated);

        if (!isMutexCreated)
        {
            return;
        }

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

        Log.CloseAndFlush();
        _mutex?.Dispose();
        factory.Dispose();
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace()
                .UseReactiveUI(rxui =>
                {
                    rxui.WithExceptionHandler(GlobalExceptionHandler.GetObservable());
                });
    }
}
