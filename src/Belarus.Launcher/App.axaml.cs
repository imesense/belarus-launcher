using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using Belarus.Launcher.Core.FileHashVerification;
using Belarus.Launcher.Core.Logger;
using Belarus.Launcher.Core.Manager;
using Belarus.Launcher.Core.Services;
using Belarus.Launcher.Core.Storage;
using Belarus.Launcher.Injection;
using Belarus.Launcher.Services;
using Belarus.Launcher.ViewModels;
using Belarus.Launcher.Views;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;

namespace Belarus.Launcher;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider = ConfigureServices()
        .BuildServiceProvider();

    private static ServiceCollection ConfigureServices()
    {
        var services = new ServiceCollection();
        var pathLog = Path.Combine(DirectoryStorage.LauncherLogs, FileNameStorage.LauncherLog);
        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(LogManager.CreateLogger(pathLog)));
        services.AddTransient<IConfiguration>(x =>
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<App>()
                .Build();
            return configuration;
        });

        services.AddPresentationServices();
        services.AddValidators();
        services.AddManagers();
        services.AddServices();

        services.AddTransient<IHashProvider, Md5HashProvider>();
        services.AddTransient<IWebsiteLauncher, WebsiteLauncher>();
        services.AddTransient<HashChecker>();
        services.AddSingleton<ILauncherStorage, MemoryLauncherStorage>();
        services.AddSingleton<ViewModelLocator>();

        return services;
    }

    public override void Initialize()
    {
        var logger = _serviceProvider.GetRequiredService<ILogger<App>>();

        try
        {
            AvaloniaXamlLoader.Load(this);
        }
        catch (Exception exception)
        {
            logger.LogCritical("{Message}", exception.Message);
            logger.LogInformation("{StackTrace}", exception.StackTrace);
            throw;
        }
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var initializerManager = _serviceProvider.GetRequiredService<InitializerManager>();
            var userManager = _serviceProvider.GetRequiredService<UserManager>();
            UserManager.MigratorSettings();

            var splashScreenManager = _serviceProvider.GetRequiredService<ISplashScreenManager>();
            splashScreenManager.MaxProgress = 4;

            await userManager.LoadAsync(splashScreenManager.CancellationToken);
            initializerManager.InitializeLocale();

            var mainViewModel = _serviceProvider.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };
            desktop.MainWindow.Show();

            try
            {
                mainViewModel.ShowSplashScreenImpl();

                await initializerManager.InitializeAsync(splashScreenManager);
                await mainViewModel.InitializeAsync(splashScreenManager);
            }
            catch (TaskCanceledException)
            {
                desktop.Shutdown();
                return;
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}
