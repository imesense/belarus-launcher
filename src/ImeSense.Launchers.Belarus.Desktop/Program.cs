using System.Globalization;

using Avalonia;
using Avalonia.ReactiveUI;

using ImeSense.Launchers.Belarus.Helpers;
using ImeSense.Launchers.Belarus.Core.Logger;
using ImeSense.Launchers.Belarus.Core.Storage;

using Serilog;

namespace ImeSense.Launchers.Belarus.Desktop;

internal class Program
{
    private const string _mutexName = "Belarus.Launcher.Desktop";

    private static Mutex? _mutex;

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var pathLog = Path.Combine(DirectoryStorage.LauncherLogs, FileNameStorage.LauncherLog);
        Log.Logger = LogManager.CreateLogger(pathLog);
        var isMutexCreated = false;
        try {
            _mutex = new Mutex(initiallyOwned: false, _mutexName, out isMutexCreated);
        } catch (Exception exception) {
            Log.Error("{Message} \n {StackTrace}", exception.Message, exception.StackTrace);
        }
        if (!isMutexCreated) {
            return;
        }

        try {
#if DEBUG
            StartApp(args);
#else
            try {
                StartApp(args);
            } catch (Exception exception) {
                Log.Error("{Message} \n {StackTrace}", exception.Message, exception.StackTrace);
                throw;
            }
#endif
        } finally {
            Log.CloseAndFlush();
            _mutex?.Dispose();
        }
    }

    private static void StartApp(string[] args)
    {
        Log.Information("Start SBLauncher");
        Log.Information(InformationPrinter.GetOsInfo());
        Log.Information(InformationPrinter.GetApplicationInfo());

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}
