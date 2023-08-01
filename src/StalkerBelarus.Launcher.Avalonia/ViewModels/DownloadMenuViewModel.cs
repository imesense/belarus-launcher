using System.Reactive;
using System.Reactive.Linq;

using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using StalkerBelarus.Launcher.Core.Manager;
using StalkerBelarus.Launcher.Core.Services;

namespace StalkerBelarus.Launcher.Avalonia.ViewModels; 

public class DownloadMenuViewModel : ViewModelBase {
    private readonly ILogger<DownloadMenuViewModel> _logger;
    private readonly IWindowManager _windowManager;
    private readonly IDownloadResourcesService _downloadResourcesService;

    // The cancellation token is used to interrupt the loader at any time
    private readonly CancellationTokenSource? _tokenSource = null;
    
    public ReactiveCommand<Unit, Unit> StartDownload { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> Close { get; private set; } = null!;

    [Reactive] public int Progress { get; set; } = 0;

    public DownloadMenuViewModel(ILogger<DownloadMenuViewModel> logger, IWindowManager windowManager,
        IDownloadResourcesService downloadResourcesService) {
        _logger = logger;
        _windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
        _downloadResourcesService = downloadResourcesService ?? throw new ArgumentNullException(nameof(downloadResourcesService));

        SetupCommands();
    }
    
    private void SetupCommands() {
        StartDownload = ReactiveCommand.CreateFromTask(DownloadsImplAsync);
        Close = ReactiveCommand.Create(CloseImpl);
        
        StartDownload.ThrownExceptions.Merge(Close.ThrownExceptions)
            .Throttle(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler)
            .Subscribe(OnCommandException);
    }

    private void CloseImpl() {
        _tokenSource?.Cancel();
        _windowManager.Close();
    }

    private async Task DownloadsImplAsync() {
        var progress = new Progress<int>(percentage => {
            Progress = percentage;
        });

        await _downloadResourcesService.DownloadsAsync(progress, _tokenSource);
    }

    private void OnCommandException(Exception exception) 
        => _logger.LogError("{Message}", exception.Message);
}
