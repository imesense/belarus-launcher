using System.Collections.ObjectModel;

using ReactiveUI.Fody.Helpers;

using ImeSense.Launchers.Belarus.Core.Manager;

namespace ImeSense.Launchers.Belarus.ViewModels;

public partial class NewsSliderViewModel : ViewModelBase {
    [Reactive] public NewsViewModel? SelectedNewsViewModel { get; private set; }
    [Reactive] public int NumPage { get; private set; } = 0;
    private readonly DownloadManager _downloadService;

    public ObservableCollection<NewsViewModel> News { get; private set; } = new();

    public NewsSliderViewModel(DownloadManager downloadService) {
        _downloadService = downloadService;
        LoadNews();
        
        SetupBinding();
        SetupCommands();
    }

    private void LoadNews() {
        foreach (var content in _downloadService.GetNewsList()) {
            News.Add(new NewsViewModel(content.Title, content.Description));
        }
    }
}
