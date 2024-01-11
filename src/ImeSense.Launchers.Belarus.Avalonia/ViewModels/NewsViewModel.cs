using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using ImeSense.Launchers.Belarus.Core.Models;

namespace ImeSense.Launchers.Belarus.Avalonia.ViewModels;

public class NewsViewModel : ReactiveObject {
    [Reactive] public string Title { get; set; }
    [Reactive] public string Description { get; set; }

    public NewsViewModel(string title, string description) {
        Title = title;
        Description = description;
    }

    public NewsViewModel(NewsContent newsContent) {
        Title = newsContent.Title;
        Description = newsContent.Description;
    }

#if DEBUG
    public NewsViewModel() {
        Title = null!;
        Description = null!;
    }
#endif
}
