using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Splat;

using StalkerBelarus.Launcher.Core.Manager;
using StalkerBelarus.Launcher.Manager;

namespace StalkerBelarus.Launcher;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {
    private readonly IHost _host;

    public App() {
        var userSettings = ConfigManager.LoadSettings();

        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) => {
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
            }).Build();

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

    protected override async void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);

        await _host.StartAsync();

        MainWindow = _host.Services.GetRequiredService<MainWindow>();
        MainWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e) {
        await _host.StopAsync();
        _host.Dispose();

        base.OnExit(e);
    }
}
