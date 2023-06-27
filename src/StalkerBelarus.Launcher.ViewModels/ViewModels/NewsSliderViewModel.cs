using System.Collections.ObjectModel;

using DynamicData;

using ReactiveUI.Fody.Helpers;

using StalkerBelarus.Launcher.ViewModels.Manager;

namespace StalkerBelarus.Launcher.ViewModels;

public partial class NewsSliderViewModel : ReactiveObject {
    [Reactive] public NewsViewModel? SelectedNewsViewModel { get; private set; }
    [Reactive] public int NumPage { get; private set; } = 0;
    private readonly DownloadManager _downloadService;

    public ObservableCollection<NewsViewModel> News { get; private set; } = new();

    public NewsSliderViewModel(DownloadManager downloadService) {
        _downloadService = downloadService;
        News.AddRange(_downloadService.GetNewsList());
        
        SetupBinding();
        SetupCommands();
    }
}
