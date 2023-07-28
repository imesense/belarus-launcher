using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;

using StalkerBelarus.Launcher.Avalonia.Manager;
using StalkerBelarus.Launcher.Avalonia.ViewModels;
using StalkerBelarus.Launcher.Avalonia.Views;
using StalkerBelarus.Launcher.Core;
using StalkerBelarus.Launcher.Core.Manager;
using StalkerBelarus.Launcher.Core.Validators;

namespace StalkerBelarus.Launcher.Avalonia;

public partial class App : Application {
    private readonly IServiceProvider _serviceProvider;

    public App() {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(Path.Combine(FileLocations.LogsDirectory, "sblauncher_report.txt"), 
                rollingInterval: RollingInterval.Infinite)
            .CreateLogger();
        
        _serviceProvider = ConfigureServices()
            .BuildServiceProvider();
    }
    
    private IServiceCollection ConfigureServices() {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder =>
            loggingBuilder.AddSerilog(dispose: true));
        services.AddSingleton<IWindowManager, WindowManager>();
        services.AddSingleton(ConfigManager.LoadSettings());
        services.AddTransient<GameDirectoryValidator>();

        services.AddTransient<AuthorizationViewModel>();
        services.AddTransient<LauncherViewModel>();
        services.AddTransient<DownloadMenuViewModel>();
        services.AddTransient<GameMenuViewModel>();
        services.AddSingleton<MainWindowViewModel>();

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
            desktop.MainWindow = new MainWindow() {
                DataContext = _serviceProvider.GetRequiredService<MainWindowViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
