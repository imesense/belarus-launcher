using StalkerBelarus.Launcher.ViewModels.Manager;
using System.Reactive;
using System.Reactive.Linq;

namespace StalkerBelarus.Launcher.ViewModels
{
    public class AuthorizationViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly IWindowManager _windowManager;
        private readonly LauncherViewModel _launcherViewModel;

        public ReactiveCommand<Unit, Unit> Next { get; private set; } = null!;
        public ReactiveCommand<Unit, Unit> Close { get; private set; } = null!;
        public string? UrlPathSegment => throw new NotImplementedException();
        public IScreen HostScreen { get; set; } = null!;

        public AuthorizationViewModel(IWindowManager windowManager, LauncherViewModel launcherViewModel)
        {
            _windowManager = windowManager;

            if (launcherViewModel is null)
            {
                throw new ArgumentNullException(nameof(launcherViewModel));
            }
            
            _launcherViewModel = launcherViewModel;
            _launcherViewModel.HostScreen = HostScreen;

            SetupBinding();
        }

        private void SetupBinding()
        {
            Next = ReactiveCommand.CreateFromTask(async () => await NextImpl());
            Close = ReactiveCommand.Create(() => _windowManager.Close());
        }

        private IObservable<Unit> NextImpl()
        {
            if (HostScreen is null)
            {
                throw new ArgumentNullException(nameof(HostScreen));
            }

            HostScreen.Router.Navigate.Execute(_launcherViewModel);

            return Observable.Return(Unit.Default);
        }
    }
}
