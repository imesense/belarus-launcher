using Microsoft.Extensions.Logging;

namespace StalkerBelarus.Launcher.Avalonia.ViewModels;

public class GameMenuViewModel : ViewModelBase {
    private readonly ILogger<GameMenuViewModel> _logger;

    public GameMenuViewModel(ILogger<GameMenuViewModel> logger) {
        _logger = logger;
    }
}
