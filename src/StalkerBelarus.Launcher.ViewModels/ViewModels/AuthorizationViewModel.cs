using System.Reactive.Linq;

using ReactiveUI.Fody.Helpers;

using StalkerBelarus.Launcher.Core.Manager;
using StalkerBelarus.Launcher.Core.Models;

namespace StalkerBelarus.Launcher.ViewModels {
    public class AuthorizationViewModel : ReactiveObject, IRoutableViewModel {
        private readonly IWindowManager _windowManager;
        private readonly LauncherViewModel _launcherViewModel;
        private UserSettings _userSettings;
        
        [Reactive] public string UserName { get; set; } = string.Empty;

        public ReactiveCommand<Unit, Unit> Next { get; private set; } = null!;
        public ReactiveCommand<Unit, Unit> Close { get; private set; } = null!;

        public string? UrlPathSegment { get; set; } = "";

        public IScreen HostScreen { get; set; } = null!;

        public AuthorizationViewModel(IWindowManager windowManager, LauncherViewModel launcherViewModel, UserSettings userSettings) {
            _windowManager = windowManager;

            if (launcherViewModel is null) {
                throw new ArgumentNullException(nameof(launcherViewModel));
            }

            _userSettings = userSettings;

            _launcherViewModel = launcherViewModel;
            _launcherViewModel.HostScreen = HostScreen;

            SetupBinding();
        }

        private void SetupBinding() {
            var canCreateUser = this.WhenAnyValue(x => x.UserName,
                (nickname) => !string.IsNullOrWhiteSpace(nickname) && nickname.Length <= 22)
                .ObserveOn(RxApp.MainThreadScheduler)
                .DistinctUntilChanged();

            Next = ReactiveCommand.CreateFromTask(async () => await NextImpl(), canCreateUser);
            Close = ReactiveCommand.Create(_windowManager.Close);
        }

        private IObservable<Unit> NextImpl() {
            if (HostScreen is null) {
                throw new ArgumentNullException(nameof(HostScreen));
            }
            if (string.IsNullOrWhiteSpace(UserName)) {
                throw new Exception("Имя пользователя не введено!");
            }

            _userSettings.UserName = UserName;
            ConfigManager.SaveSettings(_userSettings);

            HostScreen.Router.Navigate.Execute(_launcherViewModel);

            return Observable.Return(Unit.Default);
        }
    }
}
