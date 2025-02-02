using System.Reactive;
using System.Reactive.Linq;

using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.Core.Services;
using ImeSense.Launchers.Belarus.Core.Storage;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ImeSense.Launchers.Belarus.ViewModels;

public class DownloadMenuViewModel : ReactiveObject
{
    private readonly ILogger<DownloadMenuViewModel>? _logger;
    private readonly IApplicationLocaleManager _localeManager;
    private readonly IWindowManager _windowManager;
    private readonly IDownloadResourcesService _downloadResourcesService;
    private readonly ILauncherStorage _launcherStorage;
    private CancellationTokenSource _cts;

    public CancellationToken CancellationToken { get; private set; }
    public ReactiveCommand<LauncherViewModel, Unit> StartDownload { get; private set; }
    public ReactiveCommand<Unit, Unit> Pause { get; private set; }
    public ReactiveCommand<Unit, Unit> Close { get; private set; }

    [Reactive] public int DownloadProgress { get; set; } = 0;
    [Reactive] public string StatusProgress { get; set; } = string.Empty;
    [Reactive] public string DownloadFileName { get; set; } = string.Empty;

    // Overall progress status
    [Reactive] public bool IsProgress { get; set; }

    //Download status
    [Reactive] public bool IsDownload { get; set; }

    public DownloadMenuViewModel(ILogger<DownloadMenuViewModel>? logger,
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
        _cts = null!;

        SetupCommands();

        StartDownload = StartDownload ?? throw new NullReferenceException(nameof(StartDownload));
        Pause = Pause ?? throw new NullReferenceException(nameof(Pause));
        Close = Close ?? throw new NullReferenceException(nameof(Close));
    }

    public async Task UpdateAsync(LauncherViewModel launcherViewModel)
    {
        await StartDownload.Execute(launcherViewModel);
    }

    private void SetupCommands()
    {
        var isGitHubConnection = this.WhenAnyValue(x => x._launcherStorage.IsCheckGitHubConnection);

        StartDownload = ReactiveCommand.CreateFromTask<LauncherViewModel>(DownloadsImplAsync, isGitHubConnection);
        Pause = ReactiveCommand.Create(PauseImpl);
        Close = ReactiveCommand.Create(CloseImpl);

        StartDownload.ThrownExceptions.Merge(Close.ThrownExceptions)
            .Throttle(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler)
            .Subscribe(OnCommandException);
    }

    private void PauseImpl()
    {
        ClearData();
        _cts?.Cancel();
    }

    private void ClearData()
    {
        DownloadProgress = 0;
        DownloadFileName = string.Empty;
        IsProgress = false;
        IsDownload = false;
    }

    private void CloseImpl()
    {
        PauseImpl();

        _windowManager.Close();
    }

    private async Task DownloadsImplAsync(LauncherViewModel launcherViewModel)
    {
        _cts = new();
        CancellationToken = _cts.Token;
        ClearData();

        IProgress<int> progress = new Progress<int>(percentage =>
        {
            DownloadProgress = percentage;
        });

        IsProgress = true;
        StatusProgress = _localeManager.GetStringByKey("LocalizedStrings.IntegrityChecking");

        var filesDownload = await _downloadResourcesService.GetFilesForDownloadAsync(progress, CancellationToken);

        if (CancellationToken.IsCancellationRequested)
        {
            launcherViewModel.SelectMenu();
            PauseImpl();
            return;
        }

        if (filesDownload is not null && filesDownload.Any())
        {
            var countFiles = filesDownload.Count;
            var numberFile = 0;
            IsDownload = true;
            foreach (var file in filesDownload)
            {
                numberFile++;
                StatusProgress = _localeManager.GetStringByKey("LocalizedStrings.Files") +
                                 $": {numberFile} / {countFiles}";
                DownloadFileName = Path.GetFileName(file.Key);
                await _downloadResourcesService.DownloadAsync(file.Key, file.Value, progress, CancellationToken);

                if (CancellationToken.IsCancellationRequested)
                {
                    PauseImpl();
                    launcherViewModel.SelectMenu();
                    return;
                }

                progress.Report(0);
            }
        }

        progress.Report(0);
        ClearData();

        launcherViewModel.SelectMenu();
    }

    private void OnCommandException(Exception exception)
    {
        ClearData();

        _logger?.LogError("{Message}", exception.Message);
        _logger?.LogError("{Message}", exception.StackTrace);
    }
}
