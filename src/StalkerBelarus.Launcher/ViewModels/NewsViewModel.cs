using ReactiveUI.Fody.Helpers;

namespace StalkerBelarus.Launcher.ViewModels;

public class NewsViewModel : ReactiveObject
{
    #region Public Properties
    [Reactive] public string Title { get; private set; } = string.Empty;
    [Reactive] public string Description { get; private set; } = string.Empty;
    #endregion

    #region Constructor
    public NewsViewModel(string title, string description) 
    {
        Title = title;
        Description = description;
    }
    #endregion
}
