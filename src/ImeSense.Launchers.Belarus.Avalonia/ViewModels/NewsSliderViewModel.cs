using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;

using ImeSense.Launchers.Belarus.Avalonia.Helpers;
using ImeSense.Launchers.Belarus.Avalonia.Services;
using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.Core.Models;
using ImeSense.Launchers.Belarus.Core.Storage;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ImeSense.Launchers.Belarus.Avalonia.ViewModels;

public class NewsSliderViewModel : ReactiveObject
{
    private readonly ILogger<NewsSliderViewModel> _logger;
    private readonly ViewModelLocator _viewModelLocator;
    private readonly ILauncherStorage _launcherStorage;

    [Reactive] public int NumPage { get; set; }
    [Reactive] public NewsViewModel? SelectedNewsViewModel { get; private set; }
    [Reactive] public LinkViewModel LinkViewModel { get; set; }
    [Reactive] public ObservableCollection<NewsViewModel>? News { get; set; }

    public ReactiveCommand<Unit, Unit> GoNext { get; set; } = null!;
    public ReactiveCommand<Unit, Unit> GoBack { get; set; } = null!;
    public UserManager UserManager { get; set; }

    public NewsSliderViewModel(ILogger<NewsSliderViewModel> logger, ViewModelLocator viewModelLocator,
        UserManager userManager, ILauncherStorage launcherStorage)
    {
        logger.LogInformation("NewsSliderViewModel CTOR");
        _logger = logger;
        _viewModelLocator = viewModelLocator;
        UserManager = userManager;
        _launcherStorage = launcherStorage;
        LinkViewModel = viewModelLocator.LinkViewModel;
        SetupBinding();
        SetupCommands();

        this.WhenAnyValue(x => x.UserManager.UserSettings!.Locale)
            .Subscribe(ReloadNews);

        this.WhenAnyValue(x => x._launcherStorage.NewsContents)
            .Select(news => news != null && news.Any())
            .Subscribe((n) => _logger.LogInformation("NewsContents --------------------------------------------------"));

        this.WhenAnyValue(x => x._launcherStorage.NewsContents[0])
            .Subscribe((n) => _logger.LogInformation("NewsContents[0] --------------------------------------------------"));
    }

    private void ReloadNews(Locale? locale)
    {
        _logger.LogInformation("Call ReloadNews() method");

        if (_launcherStorage.NewsContents is null) {
            _logger.LogError("News content is null");
            return;
        }

        var news = _launcherStorage.NewsContents
            .FirstOrDefault(x => x.Locale != null && locale != null && x.Locale.Key.Equals(locale.Key));
        if (news is not null) {
            SetNews(news.NewsContents!);
        } else {
            _logger.LogError("News collection is empty");
        }
    }

    public NewsSliderViewModel()
    {
        ExceptionHelper.ThrowIfEmptyConstructorNotInDesignTime($"{nameof(NewsSliderViewModel)}");

        _logger = null!;

        LinkViewModel = null!;
        _viewModelLocator = null!;
        UserManager = null!;
        _launcherStorage = null!;
    }

    private void SetupCommands()
    {
        var canExecuteBack = this.WhenAnyValue(x => x.NumPage,
                (numPage) => numPage != 0)
            .ObserveOn(RxApp.MainThreadScheduler);
        var canExecuteNext = this.WhenAnyValue(x => x.NumPage,
                (numPage) => News != null && numPage != News.Count - 1 && News.Count != 0)
            .ObserveOn(RxApp.MainThreadScheduler);

        GoNext = ReactiveCommand.Create(GoNextImpl, canExecuteNext);
        GoBack = ReactiveCommand.Create(GoBackImpl, canExecuteBack);

        GoNext.ThrownExceptions.Merge(GoBack.ThrownExceptions)
            .Throttle(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler)
            .Subscribe(OnCommandException);
    }

    private void SetupBinding()
    {
        this.WhenAnyValue(x => x.NumPage)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Where(x => News != null && x >= 0 && x < News.Count)
            .Subscribe(x => {
                if (News is not null && News.Any()) {
                    SelectedNewsViewModel = News[x];
                }
            });
    }

    private void GoNextImpl()
    {
        if (News is null) {
            return;
        }
        if (NumPage < News.Count - 1) {
            NumPage++;
        }
    }

    private void GoBackImpl()
    {
        if (NumPage > 0) {
            NumPage--;
        }
    }

    public void SetNews(IEnumerable<NewsContent> newsContents)
    {
        News = [];

        foreach (var content in newsContents) {
            News.Add(new NewsViewModel(content!.Title, content.Description));
        }

        NumPage = News.Count - 1;
        SelectedNewsViewModel = News[NumPage];
    }

    private void OnCommandException(Exception exception)
        => _logger.LogError("{Message}", exception.Message);
}
