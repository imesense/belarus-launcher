using ReactiveUI.Fody.Helpers;

namespace StalkerBelarus.Launcher.ViewModels;

public class NewsViewModel : ReactiveObject {
    [Reactive] public string Title { get; init; } = string.Empty;
    [Reactive] public string Description { get; init; } = string.Empty;

    public NewsViewModel(string title, string description) {
        Title = title;
        Description = description;
    }
}
