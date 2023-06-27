using System.Diagnostics;
using System.Reactive.Linq;

namespace StalkerBelarus.Launcher.ViewModels;

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
        
        GoVk = ReactiveCommand.Create(GoVkImpl);
        GoTg = ReactiveCommand.Create(GoTgImpl);
        GoApPro = ReactiveCommand.Create(GoApProImpl);
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

    private void GoVkImpl() {
        Process.Start(new ProcessStartInfo("https://vk.com/stalker_belarus") { UseShellExecute = true });
    }

    private void GoTgImpl() {
        Process.Start(new ProcessStartInfo("https://t.me/stalkerbelarus") { UseShellExecute = true });
    }

    private void GoApProImpl() {
        Process.Start(new ProcessStartInfo("https://ap-pro.ru/forums/topic/3923-stalker-belarus/") { UseShellExecute = true });
    }
}
