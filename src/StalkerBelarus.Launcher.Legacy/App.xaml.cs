using Microsoft.Extensions.DependencyInjection;

using Splat;

using StalkerBelarus.Launcher.Core.Manager;
using StalkerBelarus.Launcher.Manager;

namespace StalkerBelarus.Launcher;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {
    private readonly IServiceProvider _serviceProvider;

    public App() {
        var userSettings = ConfigManager.LoadSettings();

        var services = new ServiceCollection();
        services.AddSingleton<DownloadManager>();
        services.AddSingleton(userSettings);
        services.AddSingleton<IWindowManager, WindowManager>();

        services.AddSingleton<AuthorizationViewModel>();
        services.AddSingleton<LauncherViewModel>();
        services.AddSingleton<MenuViewModel>();
        services.AddSingleton<StartGameViewModel>();
        services.AddSingleton<NewsSliderViewModel>();

        services.AddSingleton<IScreen, MainViewModel>();
        services.AddSingleton((services) => new MainWindow() {
            DataContext = services.GetRequiredService<IScreen>()
        });

        _serviceProvider = services.BuildServiceProvider();

        Locator.CurrentMutable.InitializeReactiveUI();
        Locator.CurrentMutable.InitializeSplat();

        RegisterPages();
    }

    private static void RegisterPages() {
        Locator.CurrentMutable.Register<IViewFor<AuthorizationViewModel>>(
            () => new AuthorizationView());
        Locator.CurrentMutable.Register<IViewFor<LauncherViewModel>>(
            () => new LauncherView());
        Locator.CurrentMutable.Register<IViewFor<StartGameViewModel>>(
            () => new StartGameView());
    }

    protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);

        MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        MainWindow.Show();
    }
}
