using Splat;
using StalkerBelarus.Launcher.Views;

namespace StalkerBelarus.Launcher.ViewModels
{
    /// <summary>
    /// Main view model
    /// </summary>
    public class MainViewModel : ReactiveObject, IScreen
    {
        public RoutingState Router { get; }

        #region Constructor
        public MainViewModel(IMutableDependencyResolver? dependencyResolver = null)
        {
            dependencyResolver ??= Locator.CurrentMutable;

            // Bind
            RegisterParts(dependencyResolver);
            Router = new RoutingState();

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

        private void RegisterParts(IMutableDependencyResolver dependencyResolver)
        {
            dependencyResolver.RegisterConstant(this, typeof(IScreen));

            dependencyResolver.Register(() => new AuthorizationView(), typeof(IViewFor<AuthorizationViewModel>));
            dependencyResolver.Register(() => new LauncherView(), typeof(IViewFor<LauncherViewModel>));
        }
    }
}