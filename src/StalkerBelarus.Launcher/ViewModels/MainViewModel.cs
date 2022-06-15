namespace StalkerBelarus.Launcher.ViewModels;

/// <summary>
/// Main view model
/// </summary>
public class MainViewModel : ReactiveObject, IScreen
{
    public RoutingState Router { get; } = new();

    #region Constructor
    public MainViewModel()
    {
        ShowAuthorization();
    }
    #endregion

    private void ShowLauncher()
    {
        Router.Navigate.Execute(new LauncherViewModel(this)).Subscribe();
    }

    private void ShowAuthorization()
    {
        Router.Navigate.Execute(new AuthorizationViewModel(this)).Subscribe();
    }
}