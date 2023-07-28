using System.Reactive;
using System.Reactive.Linq;

using Microsoft.Extensions.Logging;
using ReactiveUI;

using StalkerBelarus.Launcher.Core.Manager;

namespace StalkerBelarus.Launcher.Avalonia.ViewModels; 

public class DownloadMenuViewModel : ViewModelBase {
    private readonly ILogger<DownloadMenuViewModel> _logger;
    private readonly IWindowManager _windowManager;

    public ReactiveCommand<Unit, Unit> StartDownload { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> Close { get; private set; } = null!;

    public DownloadMenuViewModel(ILogger<DownloadMenuViewModel> logger, IWindowManager windowManager) {
        _logger = logger;
        _windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
        
        SetupCommands();
    }
    
    private void SetupCommands() {
        StartDownload = ReactiveCommand.CreateFromTask(DownloadsImplAsync);
        Close = ReactiveCommand.Create(_windowManager.Close);
        
        StartDownload.ThrownExceptions.Merge(Close.ThrownExceptions)
            .Throttle(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler)
            .Subscribe(OnCommandException);
    }

    private async Task DownloadsImplAsync() {
        throw new NotImplementedException();
    }

    private void OnCommandException(Exception exception) 
        => _logger.LogError("{Message}", exception.Message);
}
