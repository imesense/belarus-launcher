using System.Diagnostics;
using System.Net.Http.Headers;

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
using StalkerBelarus.Launcher.Core.FileHashVerification;
using StalkerBelarus.Launcher.Core.Manager;
using StalkerBelarus.Launcher.Core.Services;
using StalkerBelarus.Launcher.Core.Validators;

namespace StalkerBelarus.Launcher.Avalonia;

public partial class App : Application {
    private readonly IServiceProvider _serviceProvider;

    public App() {
        var pathLog = Path.Combine(FileLocations.LogsDirectory, FileNamesStorage.Log);
        try {
            if (!File.Exists(pathLog)) {
                File.Delete(pathLog);
            }
        } catch (Exception exception) {
            Debug.WriteLine(exception.Message);
        } finally {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(pathLog, rollingInterval: RollingInterval.Infinite)
                .WriteTo.Debug()
                .CreateLogger();
        
            _serviceProvider = ConfigureServices()
                .BuildServiceProvider();
        }
    }
    
    private IServiceCollection ConfigureServices() {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder =>
            loggingBuilder.AddSerilog(dispose: true));
        services.AddSingleton<IWindowManager, WindowManager>();
        services.AddSingleton(ConfigManager.LoadSettings());
        services.AddTransient<GameDirectoryValidator>();
        services.AddHttpClient<IGitHubApiService, GitHubApiService>()
            .ConfigureHttpClient(ConfigureClient);
        services.AddHttpClient<IFileDownloadManager, FileDownloadManager>()
            .ConfigureHttpClient(ConfigureClient);
        services.AddTransient<IHashProvider, Md5HashProvider>();
        services.AddTransient<HashChecker>();
        services.AddTransient<IDownloadResourcesService, DownloadResourcesService>();
        services.AddTransient<IWebsiteLauncher, WebsiteLauncher>();
        services.AddTransient<AuthorizationViewModel>();
        services.AddTransient<LinkViewModel>();
        services.AddTransient<NewsSliderViewModel>();
        services.AddSingleton<LauncherViewModel>();
        services.AddTransient<DownloadMenuViewModel>();
        services.AddTransient<GameMenuViewModel>();
        services.AddSingleton<StartGameViewModel>();
        services.AddSingleton<MainWindowViewModel>();

        return services;
    }

    private static void ConfigureClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri("https://api.github.com/repos/Belarus-Mod/Mod-Data/releases/latest");
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
        httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
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
