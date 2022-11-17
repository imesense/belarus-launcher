using Splat;
using StalkerBelarus.Launcher.ViewModels;
using StalkerBelarus.Launcher.Views;

namespace StalkerBelarus.Launcher;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    #region Constructor
    public App()
    {
        RegisterPages();
    }
    #endregion

    private void RegisterPages()
    {
        Locator.CurrentMutable.RegisterConstant<IScreen>(new MainViewModel());
        Locator.CurrentMutable.Register<IViewFor<AuthorizationViewModel>>(
            () => new AuthorizationView());
        Locator.CurrentMutable.Register<IViewFor<LauncherViewModel>>(
            () => new LauncherView());
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainView = new MainWindow()
        {
            DataContext = Locator.Current.GetService<IScreen>()
        };

        mainView.Show();
    }
}
