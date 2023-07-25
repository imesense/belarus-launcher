using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using Microsoft.Extensions.DependencyInjection;

using StalkerBelarus.Launcher.Avalonia.Views;

namespace StalkerBelarus.Launcher.Avalonia;

public partial class App : Application {
    private readonly IServiceProvider _serviceProvider;

    public App() {
        _serviceProvider = ConfigureServices().BuildServiceProvider();
    }
    
    private IServiceCollection ConfigureServices() {
        var services = new ServiceCollection();

        return services;
    }

    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted() {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
