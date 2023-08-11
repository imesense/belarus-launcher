using System.Reactive;
using System.Reactive.Linq;

using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using StalkerBelarus.Launcher.Avalonia.Assets;
using StalkerBelarus.Launcher.Core.Manager;
using StalkerBelarus.Launcher.Core.Services;

namespace StalkerBelarus.Launcher.Avalonia.ViewModels; 

public class DownloadMenuViewModel : ViewModelBase {
    private readonly ILogger<DownloadMenuViewModel> _logger;
    private readonly IWindowManager _windowManager;
    private readonly IDownloadResourcesService _downloadResourcesService;

    // The cancellation token is used to interrupt the loader at any time
    private readonly CancellationTokenSource? _tokenSource = null;
    
    public ReactiveCommand<LauncherViewModel, Unit> StartDownload { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> Close { get; private set; } = null!;

    [Reactive] public int DownloadProgress { get; set; } = 0;
    [Reactive] public string StatusProgress { get; set; } = string.Empty;
    [Reactive] public string DownloadFileName { get; set; } = string.Empty;
    [Reactive] public bool IsProgress { get; set; }

    public DownloadMenuViewModel(ILogger<DownloadMenuViewModel> logger, IWindowManager windowManager,
        IDownloadResourcesService downloadResourcesService) {
        _logger = logger;
        _windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
        _downloadResourcesService = downloadResourcesService ?? throw new ArgumentNullException(nameof(downloadResourcesService));

        SetupCommands();
    }

#if DEBUG
    public DownloadMenuViewModel() {
        _logger = null!;
        _windowManager = null!;
        _downloadResourcesService = null!;
    }
#endif

    public void Update(LauncherViewModel launcherViewModel) {
        StartDownload.Execute(launcherViewModel);
    }

    private void SetupCommands() {
        StartDownload = ReactiveCommand.CreateFromTask<LauncherViewModel>(DownloadsImplAsync);
        Close = ReactiveCommand.Create(CloseImpl);
        
        StartDownload.ThrownExceptions.Merge(Close.ThrownExceptions)
            .Throttle(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler)
            .Subscribe(OnCommandException);
    }

    private void CloseImpl() {
        _tokenSource?.Cancel();
        _windowManager.Close();
    }

    private async Task DownloadsImplAsync(LauncherViewModel launcherViewModel) {
        var progress = new Progress<int>(percentage => {
            DownloadProgress = percentage;
        });

        IsProgress = true;
        StatusProgress = Resources.IntegrityChecking;
        var filesDownload = await _downloadResourcesService.GetFilesForDownloadAsync(progress);
        if (filesDownload != null && filesDownload.Any()) {
            var countFiles = filesDownload.Count;
            var numberFile = 0;
            
            foreach (var file in filesDownload) {
                numberFile++;
                StatusProgress = Resources.Files + $": {numberFile} / {countFiles}";
                DownloadFileName = Path.GetFileName(file.Key);
                await _downloadResourcesService.DownloadAsync(file.Key, file.Value, progress, _tokenSource);
            }
        }
        
        launcherViewModel.SelectMenu();
    }

    private void OnCommandException(Exception exception) 
        => _logger.LogError("{Message}", exception.Message);
}
