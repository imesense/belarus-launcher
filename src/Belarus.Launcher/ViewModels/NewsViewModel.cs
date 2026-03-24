using Belarus.Launcher.Core.Models;

using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace Belarus.Launcher.ViewModels;

public partial class NewsViewModel : ReactiveObject
{
    [Reactive] public partial string Title { get; private set; }
    [Reactive] public partial string Description { get; private set; }

    public NewsViewModel(string title, string description)
    {
        Title = title;
        Description = description;
    }

    public NewsViewModel(NewsContent newsContent)
    {
        Title = newsContent.Title;
        Description = newsContent.Description;
    }
}
