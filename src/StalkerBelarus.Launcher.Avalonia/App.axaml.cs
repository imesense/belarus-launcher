using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;

using StalkerBelarus.Launcher.Avalonia.Views;

namespace StalkerBelarus.Launcher.Avalonia;

public partial class App : Application {
    private readonly IServiceProvider _serviceProvider;

    public App() {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(@"_user_/logs/sblauncher_report.txt", rollingInterval: RollingInterval.Infinite)
            .CreateLogger();
        
        _serviceProvider = ConfigureServices()
            .BuildServiceProvider();
    }
    
    private IServiceCollection ConfigureServices() {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder =>
            loggingBuilder.AddSerilog(dispose: true));
        return services;
    }
    
    public override void Initialize() {
        var logger = _serviceProvider.GetRequiredService<ILogger<App>>();
        try {
            AvaloniaXamlLoader.Load(this);
        } catch (Exception exception) {
            logger.LogCritical("{Message}", exception.Message);
            throw;
        }
    }

    public override void OnFrameworkInitializationCompleted() {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
