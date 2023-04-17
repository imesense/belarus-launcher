namespace StalkerBelarus.Launcher.ViewModels;

public class LauncherViewModel : ReactiveObject, IRoutableViewModel {
    private readonly StartGameViewModel _startGameViewModel;

    public string? UrlPathSegment => "LauncherViewModel";

    public IScreen HostScreen { get; set; } = null!;

    public MenuViewModel MenuViewModel { get; set; }

    public LauncherViewModel(MenuViewModel menuViewModel, StartGameViewModel startGameViewModel) {
        if (menuViewModel is null) {
            throw new ArgumentNullException(nameof(menuViewModel));
        }

        MenuViewModel = menuViewModel;
        MenuViewModel.LauncherViewModel = this;
        _startGameViewModel = startGameViewModel;
        _startGameViewModel.HostScreen = HostScreen;
    }

    public void StartGame() {
        HostScreen.Router.Navigate.Execute(_startGameViewModel);
    }
}
