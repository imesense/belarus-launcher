using System.Collections.ObjectModel;

using ReactiveUI.Fody.Helpers;

namespace StalkerBelarus.Launcher.ViewModels;

public partial class NewsSliderViewModel : ReactiveObject {
    [Reactive] public NewsViewModel? SelectedNewsViewModel { get; private set; }
    [Reactive] public int NumPage { get; private set; } = 0;

    public ObservableCollection<NewsViewModel> News { get; private set; } = new();

    public NewsSliderViewModel() {
        SetupBinding();
        SetupCommands();
    }
}
