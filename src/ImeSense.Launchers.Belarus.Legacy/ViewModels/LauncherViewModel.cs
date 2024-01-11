namespace ImeSense.Launchers.Belarus.ViewModels;

public class LauncherViewModel : ViewModelBase, IRoutableViewModel {
    private readonly StartGameViewModel _startGameViewModel;

    public string? UrlPathSegment => "LauncherViewModel";

    public IScreen HostScreen { get; set; } = null!;

    public MenuViewModel MenuViewModel { get; set; }
    public NewsSliderViewModel NewsSliderViewModel { get; set; }

    public LauncherViewModel(MenuViewModel menuViewModel, StartGameViewModel startGameViewModel, NewsSliderViewModel newsSliderViewModel) {
        if (menuViewModel is null) {
            throw new ArgumentNullException(nameof(menuViewModel));
        }

        MenuViewModel = menuViewModel;
        MenuViewModel.LauncherViewModel = this;
        NewsSliderViewModel = newsSliderViewModel;
        _startGameViewModel = startGameViewModel;
        _startGameViewModel.HostScreen = HostScreen;
    }

    public void StartGame() {
        HostScreen.Router.Navigate.Execute(_startGameViewModel);
    }
}
