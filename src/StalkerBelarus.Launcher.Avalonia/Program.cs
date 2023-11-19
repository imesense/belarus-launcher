using Avalonia;
using Avalonia.ReactiveUI;

using Serilog;

using StalkerBelarus.Launcher.Core.Logger;

namespace StalkerBelarus.Launcher.Avalonia;

class Program {
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) {
        try {
            LauncherLoggerFactory.CreateLogger();
            Log.Information("Start launcher");
            BuildAvaloniaApp()
                    .StartWithClassicDesktopLifetime(args);
        } catch (Exception exception) {
            Log.Error("{Message} \n {StackTrace}", exception.Message, exception.StackTrace);
            throw;
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}
