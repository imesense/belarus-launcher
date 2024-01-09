using System.Reactive;
using System.Reactive.Linq;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using StalkerBelarus.Launcher.Avalonia.Manager;
using StalkerBelarus.Launcher.Core.Manager;
using StalkerBelarus.Launcher.Core.Models;
using StalkerBelarus.Launcher.Core.Services;

namespace StalkerBelarus.Launcher.Avalonia.ViewModels;

public class DownloadMenuViewModel : ReactiveObject {
    private readonly ILogger<DownloadMenuViewModel> _logger;
    private readonly ILocaleManager _localeManager;

    private readonly IWindowManager _windowManager;
    private readonly IDownloadResourcesService _downloadResourcesService;
    private readonly UserManager _userManager;

    // The cancellation token is used to interrupt the loader at any time
    private readonly CancellationTokenSource? _tokenSource = null;
    
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
        ILocaleManager localeManager,
        IWindowManager windowManager,
        IDownloadResourcesService downloadResourcesService,
        UserManager userManager) {
        _logger = logger;
        _localeManager = localeManager;
        _windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
        _downloadResourcesService = downloadResourcesService ?? throw new ArgumentNullException(nameof(downloadResourcesService));
        _userManager = userManager;
        IsDownload = false;

        SetupCommands();
    }

#if DEBUG
    public DownloadMenuViewModel() {
        _logger = null!;
        _localeManager = null!;
        _windowManager = null!;
        _downloadResourcesService = null!;
        _userManager = null!;
    }
#endif

    public async Task UpdateAsync(LauncherViewModel launcherViewModel) {
        await StartDownload.Execute(launcherViewModel);
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
        if (_userManager is null) {
            throw new NullReferenceException("User manager object is null");
        }
        if (_userManager.UserSettings is null) {
            throw new NullReferenceException("User settings object is null");
        }
        if (_userManager.UserSettings.Locale is null) {
            throw new NullReferenceException("User settings locale object is null");
        }

        var progress = new Progress<int>(percentage => {
            DownloadProgress = percentage;
        });

        IsProgress = true;
        StatusProgress = _localeManager.GetStringByKey("LocalizedStrings.IntegrityChecking", _userManager.UserSettings.Locale.Key);

        var filesDownload = await _downloadResourcesService.GetFilesForDownloadAsync(progress);
        if (filesDownload != null && filesDownload.Any()) {
            var countFiles = filesDownload.Count;
            var numberFile = 0;
            IsDownload = true;
            foreach (var file in filesDownload) {
                numberFile++;
                StatusProgress = _localeManager.GetStringByKey("LocalizedStrings.Files", _userManager.UserSettings.Locale.Key) +
                                 $": {numberFile} / {countFiles}";
                DownloadFileName = Path.GetFileName(file.Key);
                await _downloadResourcesService.DownloadAsync(file.Key, file.Value, progress, _tokenSource);
            }
        }
        
        IsDownload = false;
        DownloadFileName = string.Empty;
        launcherViewModel.SelectMenu();
    }

    private void OnCommandException(Exception exception) 
        => _logger.LogError("{Message}", exception.Message);
}
