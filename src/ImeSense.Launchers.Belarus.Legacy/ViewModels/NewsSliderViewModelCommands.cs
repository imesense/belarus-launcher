using System.Diagnostics;
using System.Reactive.Linq;

namespace ImeSense.Launchers.Belarus.ViewModels;

public partial class NewsSliderViewModel {
    public ReactiveCommand<Unit, Unit> GoNext { get; set; } = null!;
    public ReactiveCommand<Unit, Unit> GoBack { get; set; } = null!;
    public ReactiveCommand<Unit, Unit> GoVk { get; set; } = null!;
    public ReactiveCommand<Unit, Unit> GoApPro { get; set; } = null!;
    public ReactiveCommand<Unit, Unit> GoTg { get; set; } = null!;

    private void SetupCommands() {
        var canExecuteBack = this.WhenAnyValue(x => x.NumPage, (numPage) => numPage != 0);
        var canExecuteNext = this.WhenAnyValue(x => x.NumPage, (numPage) => numPage != News.Count - 1 && News.Count != 0);

        GoNext = ReactiveCommand.Create(GoNextImpl, canExecuteNext);
        GoBack = ReactiveCommand.Create(GoBackImpl, canExecuteBack);
        
        GoVk = ReactiveCommand.Create(() => GoWebSite("https://vk.com/stalker_belarus"));
        GoTg = ReactiveCommand.Create(() => GoWebSite("https://t.me/stalkerbelarus"));
        GoApPro = ReactiveCommand.Create(() => GoWebSite("https://ap-pro.ru/forums/topic/3923-stalker-belarus/"));
    }

    private void SetupBinding() {
        var result = this.WhenAnyValue(x => x.NumPage)
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

    private static void GoWebSite(string url) {
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }
}
