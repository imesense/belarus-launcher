using DynamicData;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;

namespace StalkerBelarus.Launcher.ViewModels;

public partial class NewsSliderViewModel : ReactiveObject
{
    #region Public Properties
    [Reactive] public NewsViewModel? SelectedNewsViewModel { get; private set; }
    [Reactive] public int NumPage { get; private set; } = 0;

    public ObservableCollection<NewsViewModel> News { get; private set; } = new();
    #endregion

    #region Constructor
    public NewsSliderViewModel()
    {
        SetupBinding();
        SetupCommands();
    }
    #endregion
}
