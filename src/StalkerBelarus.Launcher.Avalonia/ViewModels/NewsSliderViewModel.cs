using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using StalkerBelarus.Launcher.Core;
using StalkerBelarus.Launcher.Core.Manager;
using StalkerBelarus.Launcher.Core.Models;
using StalkerBelarus.Launcher.Core.Services;

namespace StalkerBelarus.Launcher.Avalonia.ViewModels;

public class NewsSliderViewModel : ReactiveObject, IAsyncInitialization {
    private readonly ILogger<NewsSliderViewModel> _logger;
    private readonly IGitStorageApiService _newsService;
    private readonly UserManager _userManager;

    [Reactive] public int NumPage { get; set; }
    [Reactive] public NewsViewModel? SelectedNewsViewModel { get; private set; }
    [Reactive] public LinkViewModel LinkViewModel { get; set; }
    [Reactive] public ObservableCollection<NewsViewModel> News { get; set; } = new();
    public Task Initialization { get; }
    public ReactiveCommand<Unit, Unit> GoNext { get; set; } = null!;
    public ReactiveCommand<Unit, Unit> GoBack { get; set; } = null!;
    
    public NewsSliderViewModel(ILogger<NewsSliderViewModel> logger, IGitStorageApiService newsService, LinkViewModel linkViewModel,
        UserManager userManager) {
        _logger = logger;
        _newsService = newsService;
        LinkViewModel = linkViewModel;
        _userManager = userManager;
        SetupBinding();
        SetupCommands();
        Initialization = InitializeAsync();
    }

#if DEBUG
    public NewsSliderViewModel() {
        _logger = null!;
        _newsService = null!;
        _userManager = null!;
        LinkViewModel = null!;
        Initialization = null!;
    }
#endif

    private async Task LoadNews() {
        if (_userManager is null) {
            throw new NullReferenceException("User manager object is null");
        }
        if (_userManager.UserSettings is null) {
            throw new NullReferenceException("User settings object is null");
        }

        var locale = _userManager.UserSettings.Locale;
        var news = _newsService.DownloadJsonArrayAsync<NewsContent>($"news_content_{locale}.json");
        await foreach (var content in news) {
            News.Add(new NewsViewModel(content!.Title, content.Description));
        }
        
        NumPage = News.Count - 1;
    }

    private void SetupCommands() {
        var canExecuteBack = this.WhenAnyValue(x => x.NumPage, 
                (numPage) => numPage != 0)
            .ObserveOn(RxApp.MainThreadScheduler);
        var canExecuteNext = this.WhenAnyValue(x => x.NumPage, 
                (numPage) => numPage != News.Count - 1 && News.Count != 0)
            .ObserveOn(RxApp.MainThreadScheduler);
        
        GoNext = ReactiveCommand.Create(GoNextImpl, canExecuteNext);
        GoBack = ReactiveCommand.Create(GoBackImpl, canExecuteBack);

        GoNext.ThrownExceptions.Merge(GoBack.ThrownExceptions)
            .Throttle(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler)
            .Subscribe(OnCommandException);
    }

    private void SetupBinding() {
        this.WhenAnyValue(x => x.NumPage)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Where(x => x >= 0 && x < News.Count)
            .Subscribe(x => SelectedNewsViewModel = News[x]);
    }

    private void GoNextImpl() {
        if (NumPage < News.Count - 1) {
            NumPage++;
        }
    }

    private void GoBackImpl() {
        if (NumPage > 0) {
            NumPage--;
        }
    }

    private async Task InitializeAsync() {
        // Asynchronously initialize this instance.
        await LoadNews();
    }
    
    private void OnCommandException(Exception exception) 
        => _logger.LogError("{Message}", exception.Message);
}
