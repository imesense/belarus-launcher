using ReactiveUI.Fody.Helpers;

namespace ImeSense.Launchers.Belarus.ViewModels;

public class NewsViewModel : ViewModelBase {
    [Reactive] public string Title { get; init; } = string.Empty;
    [Reactive] public string Description { get; init; } = string.Empty;

    public NewsViewModel(string title, string description) {
        Title = title;
        Description = description;
    }
}
