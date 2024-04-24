using System.Reactive;
using System.Reactive.Linq;

using ImeSense.Launchers.Belarus.Avalonia.Helpers;
using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.Core.Services;
using ImeSense.Launchers.Belarus.Core.Storage;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ImeSense.Launchers.Belarus.Avalonia.ViewModels;

public class DownloadMenuViewModel : ReactiveObject
{
    private readonly ILogger<DownloadMenuViewModel> _logger;
    private readonly IApplicationLocaleManager _localeManager;

    private readonly IWindowManager _windowManager;
    private readonly IDownloadResourcesService _downloadResourcesService;
    private readonly ILauncherStorage _launcherStorage;
    private CancellationTokenSource _tokenSource = new();

    public ReactiveCommand<LauncherViewModel, Unit> StartDownload { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> Close { get; private set; } = null!;

    [Reactive] public int DownloadProgress { get; set; } = 0;
    [Reactive] public string StatusProgress { get; set; } = string.Empty;
    [Reactive] public string DownloadFileName { get; set; } = string.Empty;

    // Overall progress status
    [Reactive] public bool IsProgress { get; set; }

    //Download status
    [Reactive] public bool IsDownload { get; set; }

    public DownloadMenuViewModel(ILogger<DownloadMenuViewModel> logger,
        IApplicationLocaleManager localeManager,
        IWindowManager windowManager,
        IDownloadResourcesService downloadResourcesService,
        ILauncherStorage launcherStorage)
    {
        _logger = logger;
        _localeManager = localeManager;
        _windowManager = windowManager;
        _downloadResourcesService = downloadResourcesService;
        _launcherStorage = launcherStorage;
        IsDownload = false;

        SetupCommands();
    }

    public DownloadMenuViewModel()
    {
        ExceptionHelper.ThrowIfEmptyConstructorNotInDesignTime($"{nameof(DownloadMenuViewModel)}");

        _logger = null!;
        _localeManager = null!;
        _windowManager = null!;
        _downloadResourcesService = null!;
        _launcherStorage = null!;
    }

    public async Task UpdateAsync(LauncherViewModel launcherViewModel)
    {
        await StartDownload.Execute(launcherViewModel);
    }

    private void SetupCommands()
    {
        var isGitHubConnection = this.WhenAnyValue(x => x._launcherStorage.IsCheckGitHubConnection);

        StartDownload = ReactiveCommand.CreateFromTask<LauncherViewModel>(DownloadsImplAsync, isGitHubConnection);
        Close = ReactiveCommand.Create(CloseImpl);

        StartDownload.ThrownExceptions.Merge(Close.ThrownExceptions)
            .Throttle(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler)
            .Subscribe(OnCommandException);
    }

    private void CloseImpl()
    {
        _tokenSource.Cancel();
        _tokenSource.Dispose();

        _windowManager.Close();
    }

    private async Task DownloadsImplAsync(LauncherViewModel launcherViewModel)
    {
        DownloadFileName = string.Empty;
        var progress = new Progress<int>(percentage => {
            DownloadProgress = percentage;
        });

        IsProgress = true;
        StatusProgress = _localeManager.GetStringByKey("LocalizedStrings.IntegrityChecking");

        var filesDownload = await _downloadResourcesService.GetFilesForDownloadAsync(progress);
        if (filesDownload != null && filesDownload.Any()) {
            var countFiles = filesDownload.Count;
            var numberFile = 0;
            IsDownload = true;
            try {
                foreach (var file in filesDownload) {
                    numberFile++;
                    StatusProgress = _localeManager.GetStringByKey("LocalizedStrings.Files") +
                                     $": {numberFile} / {countFiles}";
                    DownloadFileName = Path.GetFileName(file.Key);
                    await _downloadResourcesService.DownloadAsync(file.Key, file.Value, progress, _tokenSource.Token);
                }
            } catch (AggregateException ae) {
                foreach (var e in ae.InnerExceptions) {
                    if (e is TaskCanceledException) {
                    } else {
                        //Console.WriteLine(e.Message);
                    }
                }
            } finally {
                _tokenSource.Cancel();
                _tokenSource.Dispose();
            }
        }

        IsDownload = false;
        DownloadFileName = string.Empty;
        launcherViewModel.SelectMenu();
    }

    private void OnCommandException(Exception exception)
        => _logger.LogError("{Message}", exception.Message);
}
