using Splat;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StalkerBelarus.Launcher.Manager;

namespace StalkerBelarus.Launcher;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly IHost _host;

    #region Constructor
    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<IWindowManager, WindowManager>();

                services.AddTransient<AuthorizationViewModel>();
                services.AddTransient<LauncherViewModel>();
                services.AddTransient<MenuViewModel>();

                services.AddSingleton<IScreen, MainViewModel>();
                services.AddSingleton((services) => new MainWindow()
                {
                    DataContext = services.GetRequiredService<IScreen>()
                });
            }).Build();

        Locator.CurrentMutable.InitializeReactiveUI();
        Locator.CurrentMutable.InitializeSplat();

        RegisterPages();
    }
    #endregion

    private static void RegisterPages()
    {
        Locator.CurrentMutable.Register<IViewFor<AuthorizationViewModel>>(
            () => new AuthorizationView());
        Locator.CurrentMutable.Register<IViewFor<LauncherViewModel>>(
            () => new LauncherView());
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        await _host.StartAsync();

        MainWindow = _host.Services.GetRequiredService<MainWindow>();
        MainWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();

        base.OnExit(e);
    }
}
