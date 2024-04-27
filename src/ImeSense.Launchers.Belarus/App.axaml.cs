using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ImeSense.Launchers.Belarus.Models;
using ImeSense.Launchers.Belarus.Services;
using ImeSense.Launchers.Belarus.ViewModels;
using ImeSense.Launchers.Belarus.Views;
using ImeSense.Launchers.Belarus.Core.FileHashVerification;
using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.Core.Storage;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;
using ImeSense.Launchers.Belarus.Injection;
using ImeSense.Launchers.Belarus.Core.Services;

namespace ImeSense.Launchers.Belarus;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    public App()
    {
        _serviceProvider = ConfigureServices()
                .BuildServiceProvider();
    }

    private static ServiceCollection ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddLogging(loggingBuilder =>
            loggingBuilder.AddSerilog(dispose: true));

        services.AddPresetationServices();
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

        try {
            AvaloniaXamlLoader.Load(this);
        } catch (Exception exception) {
            logger.LogCritical("{Message}", exception.Message);
            logger.LogInformation("{StackTrace}", exception.StackTrace);
            throw;
        }
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            var splashScreenViewModel = _serviceProvider.GetRequiredService<SplashScreenViewModel>();

            var initializerManager = _serviceProvider.GetRequiredService<InitializerManager>();
            var userManager = _serviceProvider.GetRequiredService<UserManager>();
            await userManager.LoadAsync(splashScreenViewModel.CancellationToken);
            initializerManager.InitializeLocale();

            var localeManager = _serviceProvider.GetRequiredService<IApplicationLocaleManager>();

            var mainViewModel = _serviceProvider.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = new MainWindow {
                DataContext = mainViewModel
            };
            desktop.MainWindow.Show();

            try {
                mainViewModel.ShowSplashScreenImpl(splashScreenViewModel);
                splashScreenViewModel.Progress++;
                splashScreenViewModel.InformationMessage = new InformationMessage(
                    localeManager.GetStringByKey("LocalizedStrings.Loading"),
                    localeManager.GetStringByKey("LocalizedStrings.AccessingRepository"));
                await initializerManager.InitializeAsync(splashScreenViewModel.CancellationToken);
                splashScreenViewModel.Progress++;
                splashScreenViewModel.InformationMessage = new InformationMessage(
                    localeManager.GetStringByKey("LocalizedStrings.Loading"),
                    localeManager.GetStringByKey("LocalizedStrings.DataInitialization"));
                await mainViewModel.InitializeAsync(splashScreenViewModel.CancellationToken);
                splashScreenViewModel.Progress++;
            } catch (TaskCanceledException) {
                desktop.Shutdown();
                return;
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}
