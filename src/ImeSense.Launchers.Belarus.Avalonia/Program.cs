using Avalonia;
using Avalonia.ReactiveUI;

using Serilog;

using ImeSense.Launchers.Belarus.Core.Logger;

namespace ImeSense.Launchers.Belarus.Avalonia;

class Program {
    private const string MutexName = "Belarus.Launcher.Avalonia";

    private static Mutex? _mutex;

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) {
        var isMutexCreated = false;
        try {
            _mutex = new Mutex(initiallyOwned: false, MutexName, out isMutexCreated);
        } catch {
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
            _mutex?.Dispose();
        }
    }

    private static void StartApp(string[] args) {
        LauncherLoggerFactory.CreateLogger();
        Log.Information("Start launcher");

        PrintOsInfo();

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    private static void PrintOsInfo() {
        Log.Information("OS: {0}", Environment.OSVersion);
        Log.Information("Processor architecture: {0}",
            Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE"));
        Log.Information("User / PC: {UserName} / {MachineName}", Environment.UserName,
            Environment.MachineName);
        Log.Information(".NET: {0}", Environment.Version);
        Log.Information("ProcessId: {0}", Environment.ProcessId);
        Log.Information("Processor count: {0}", Environment.ProcessorCount);
        Log.Information("Process path: {0}", Environment.ProcessPath);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}
