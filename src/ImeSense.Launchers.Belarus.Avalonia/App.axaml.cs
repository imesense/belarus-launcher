using System.Net.Http.Headers;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;

using ImeSense.Launchers.Belarus.Avalonia.Manager;
using ImeSense.Launchers.Belarus.Avalonia.ViewModels;
using ImeSense.Launchers.Belarus.Avalonia.ViewModels.Validators;
using ImeSense.Launchers.Belarus.Avalonia.Views;
using ImeSense.Launchers.Belarus.Core.FileHashVerification;
using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.Core.Models;
using ImeSense.Launchers.Belarus.Core.Services;
using ImeSense.Launchers.Belarus.Core.Storage;
using ImeSense.Launchers.Belarus.Core.Validators;

namespace ImeSense.Launchers.Belarus.Avalonia;

public partial class App : Application {
    private readonly IServiceProvider _serviceProvider;

    public App() {
        _serviceProvider = ConfigureServices()
                .BuildServiceProvider();
    }
    
    private IServiceCollection ConfigureServices() {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder =>
            loggingBuilder.AddSerilog(dispose: true));
        services.AddSingleton<IWindowManager, WindowManager>();

        services.AddTransient<GameDirectoryValidator>();
        services.AddHttpClient<IGitStorageApiService, GitHubApiService>()
            .ConfigureHttpClient(ConfigureClient);
        services.AddHttpClient<IFileDownloadManager, FileDownloadManager>()
            .ConfigureHttpClient(ConfigureClient);
        services.AddTransient<IHashProvider, Md5HashProvider>();
        services.AddTransient<HashChecker>();
        services.AddTransient<IDownloadResourcesService, DownloadResourcesService>();
        services.AddTransient<IWebsiteLauncher, WebsiteLauncher>();
        services.AddTransient<IAuthenticationValidator, AuthenticationValidator>();
        services.AddTransient<IStartGameValidator, StartGameValidator>();
        services.AddSingleton<ILauncherStorage, MemoryLauncherStorage>();
        services.AddSingleton<ILocaleManager, LocaleManager>();
        services.AddTransient<IReleaseComparerService<GitHubRelease>, ReleaseComparerService>();
        services.AddSingleton<AuthenticationViewModelValidator>();
        services.AddSingleton<StartGameViewModelValidator>();
        services.AddSingleton<UserManager>();
        services.AddSingleton<InitializerManager>();
        services.AddTransient<LinkViewModel>();
        services.AddTransient<NewsSliderViewModel>();
        services.AddSingleton<LauncherViewModel>();
        services.AddTransient<DownloadMenuViewModel>();
        services.AddTransient<GameMenuViewModel>();

        services.AddTransient<AuthorizationViewModel>();
        services.AddSingleton<StartGameViewModel>();
        services.AddSingleton<MainWindowViewModel>();

        return services;
    }

    private static void ConfigureClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri("https://api.github.com/repos/Belarus-Mod/Mod-Data/");
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
