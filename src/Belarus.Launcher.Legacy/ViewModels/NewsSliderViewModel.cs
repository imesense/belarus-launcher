using System.Collections.ObjectModel;

using Belarus.Launcher.Core.Manager;

using ReactiveUI.SourceGenerators;

namespace Belarus.Launcher.ViewModels;

public partial class NewsSliderViewModel : ViewModelBase
{
    [Reactive] public partial NewsViewModel? SelectedNewsViewModel { get; private set; }
    [Reactive] public partial int NumPage { get; private set; } = 0;

    private readonly DownloadManager _downloadService;

    public ObservableCollection<NewsViewModel> News { get; private set; } = new();

    public NewsSliderViewModel(DownloadManager downloadService)
    {
        _downloadService = downloadService;

        LoadNews();

        SetupBinding();
        SetupCommands();
    }

    private void LoadNews()
    {
        foreach (var content in _downloadService.GetNewsList())
        {
            News.Add(new NewsViewModel(content.Title, content.Description));
        }
    }
}
