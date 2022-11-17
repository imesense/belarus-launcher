using System.Reactive.Linq;

namespace StalkerBelarus.Launcher.ViewModels;

public partial class NewsSliderViewModel
{
    public IReactiveCommand? GoNext { get; private set; }
    public IReactiveCommand? GoBack { get; private set; }

    private void SetupCommands()
    {
        var canExecuteBack = this.WhenAnyValue(x => x.NumPage, (numPage) => numPage != 0);
        var canExecuteNext = this.WhenAnyValue(x => x.NumPage, (numPage) => numPage != News.Count - 1 && News.Count != 0);

        GoNext = ReactiveCommand.Create(() => GoNextImpl(), canExecuteNext);
        GoBack = ReactiveCommand.Create(() => GoBackImpl(), canExecuteBack);
    }

    private void SetupBinding()
    {
        var result = this.WhenAnyValue(x => x.NumPage)
            .Where(x => x >= 0 && x < News.Count)
            .Subscribe(x => SelectedNewsViewModel = News[x]);
    }

    private void GoNextImpl()
    {
        if (NumPage < News.Count - 1)
        {
            NumPage++;
        }
    }

    private void GoBackImpl()
    {
        if (NumPage > 0)
        {
            NumPage--;
        }
    }
}
