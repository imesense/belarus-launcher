using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using StalkerBelarus.Launcher.Core.Manager;
using StalkerBelarus.Launcher.Core.Models;
using StalkerBelarus.Launcher.Core.Validators;

namespace StalkerBelarus.Launcher.Avalonia.ViewModels.Validators;

public sealed class StartGameViewModelValidator {
    private readonly IStartGameValidator _validator;
    private readonly ILocaleManager _localeManager;
    private readonly UserSettings _userSettings;

    public StartGameViewModelValidator(IStartGameValidator validator, ILocaleManager localeManager, UserSettings userSettings) {
        _validator = validator;
        _localeManager = localeManager;
        _userSettings = userSettings;
    }
    
    public ValidationHelper EnsureIpAddressNotEmpty(StartGameViewModel startGameViewModel) =>
        startGameViewModel.ValidationRule(viewModel => viewModel.IpAddress,
            serverAddress => serverAddress != null && _validator.IsIpAddressNotEmpty(serverAddress),
            _localeManager.GetStringByKey("LocalizedStrings.IpAddressNotEntered", _userSettings.Locale));

    public ValidationHelper EnsureValidIpAddressOrUrl(StartGameViewModel startGameViewModel) =>
            startGameViewModel.ValidationRule(viewModel => viewModel.IpAddress,
                serverAddress => serverAddress != null && _validator.IsValidIpAddressOrUrl(serverAddress),
                _localeManager.GetStringByKey("LocalizedStrings.InvalidIpAddress", _userSettings.Locale));
}
