using ImeSense.Launchers.Belarus.Avalonia.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace ImeSense.Launchers.Belarus.Avalonia.Services;

public class ViewModelLocator(IServiceProvider serviceProvider) {
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public MainWindowViewModel MainWindowViewModel => _serviceProvider.GetRequiredService<MainWindowViewModel>();
    public AuthorizationViewModel AuthorizationViewModel => _serviceProvider.GetRequiredService<AuthorizationViewModel>();
    public DownloadMenuViewModel DownloadMenuViewModel => _serviceProvider.GetRequiredService<DownloadMenuViewModel>();
    public GameMenuViewModel GameMenuViewModel => _serviceProvider.GetRequiredService<GameMenuViewModel>();
    public LauncherViewModel LauncherViewModel => _serviceProvider.GetRequiredService<LauncherViewModel>();
    public LinkViewModel LinkViewModel => _serviceProvider.GetRequiredService<LinkViewModel>();
    public NewsSliderViewModel NewsSliderViewModel => _serviceProvider.GetRequiredService<NewsSliderViewModel>();
    public StartGameViewModel StartGameViewModel => _serviceProvider.GetRequiredService<StartGameViewModel>();
}
