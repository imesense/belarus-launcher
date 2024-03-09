using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using ImeSense.Launchers.Belarus.Avalonia.Helpers;
using ImeSense.Launchers.Belarus.Core.Models;
using ImeSense.Launchers.Belarus.Avalonia.Services;

namespace ImeSense.Launchers.Belarus.Avalonia.ViewModels;

public class NewsSliderViewModel : ReactiveObject {
    private readonly ILogger<NewsSliderViewModel> _logger;
    private readonly ViewModelLocator _viewModelLocator;

    [Reactive] public int NumPage { get; set; }
    [Reactive] public NewsViewModel? SelectedNewsViewModel { get; private set; }
    [Reactive] public LinkViewModel LinkViewModel { get; set; }
    [Reactive] public ObservableCollection<NewsViewModel>? News { get; set; } 

    public ReactiveCommand<Unit, Unit> GoNext { get; set; } = null!;
    public ReactiveCommand<Unit, Unit> GoBack { get; set; } = null!;

    public NewsSliderViewModel(ILogger<NewsSliderViewModel> logger, ViewModelLocator viewModelLocator) {
        logger.LogInformation("NewsSliderViewModel CTOR");
        _logger = logger;
        _viewModelLocator = viewModelLocator;

        LinkViewModel = viewModelLocator.LinkViewModel;
        SetupBinding();
        SetupCommands();
    }

    public NewsSliderViewModel() {
        ExceptionHelper.ThrowIfEmptyConstructorNotInDesignTime($"{nameof(NewsSliderViewModel)}");

        _logger = null!;

        LinkViewModel = null!;
        _viewModelLocator = null!;
    }

    private void SetupCommands() {
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

    private void SetupBinding() {
        this.WhenAnyValue(x => x.NumPage)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Where(x => News != null && x >= 0 && x < News.Count)
            .Subscribe(x => {
                if (News is not null && News.Any()) {
                    SelectedNewsViewModel = News[x];
                }
            });
    }

    private void GoNextImpl() {
        if (News is null) {
            return;
        }
        if (NumPage < News.Count - 1) {
            NumPage++;
        }
    }

    private void GoBackImpl() {
        if (NumPage > 0) {
            NumPage--;
        }
    }

    public void SetNews(IEnumerable<NewsContent> newsContents) {
        News = new ObservableCollection<NewsViewModel>();

        foreach (var content in newsContents) {
            News.Add(new NewsViewModel(content!.Title, content.Description));
        }

        NumPage = News.Count - 1;
        SelectedNewsViewModel = News[NumPage];

        if (LinkViewModel.WebResources.Count == 0) {
            LinkViewModel.Init();
        }
    }

    private void OnCommandException(Exception exception)
        => _logger.LogError("{Message}", exception.Message);
}
