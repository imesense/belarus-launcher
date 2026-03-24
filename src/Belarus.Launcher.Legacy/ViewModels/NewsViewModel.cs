using ReactiveUI.SourceGenerators;

namespace Belarus.Launcher.ViewModels;

public partial class NewsViewModel : ViewModelBase
{
    [Reactive] public partial string Title { get; private set; } = string.Empty;
    [Reactive] public partial string Description { get; private set; } = string.Empty;

    public NewsViewModel(string title, string description)
    {
        Title = title;
        Description = description;
    }
}
