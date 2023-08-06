using ReactiveUI.Fody.Helpers;

namespace StalkerBelarus.Launcher.Avalonia.ViewModels;

public class NewsViewModel : ViewModelBase {
    [Reactive] public string Title { get; set; }
    [Reactive] public string Description { get; set; }

    public NewsViewModel(string title, string description) {
        Title = title;
        Description = description;
    }

#if DEBUG
    public NewsViewModel() {
        Title = null!;
        Description = null!;
    }
#endif
}
