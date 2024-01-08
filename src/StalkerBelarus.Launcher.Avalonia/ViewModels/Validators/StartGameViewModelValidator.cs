using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using StalkerBelarus.Launcher.Core.Manager;
using StalkerBelarus.Launcher.Core.Models;
using StalkerBelarus.Launcher.Core.Validators;

namespace StalkerBelarus.Launcher.Avalonia.ViewModels.Validators;

public sealed class StartGameViewModelValidator {
    private readonly IStartGameValidator _validator;
    private readonly ILocaleManager _localeManager;
    private readonly UserManager _userManager;

    public StartGameViewModelValidator(IStartGameValidator validator, ILocaleManager localeManager, UserManager userManager) {
        _validator = validator;
        _localeManager = localeManager;
        _userManager = userManager;
    }

    public ValidationHelper EnsureIpAddressNotEmpty(StartGameViewModel startGameViewModel) {
        if (_userManager is null) {
            throw new NullReferenceException("User manager object is null");
        }
        if (_userManager.UserSettings is null) {
            throw new NullReferenceException("User settings object is null");
        }

        return startGameViewModel.ValidationRule(viewModel => viewModel.IpAddress,
            serverAddress => serverAddress != null && _validator.IsIpAddressNotEmpty(serverAddress),
            _localeManager.GetStringByKey("LocalizedStrings.IpAddressNotEntered", _userManager.UserSettings.Locale));
    }

    public ValidationHelper EnsureValidIpAddressOrUrl(StartGameViewModel startGameViewModel) {
        if (_userManager is null) {
            throw new NullReferenceException("User manager object is null");
        }
        if (_userManager.UserSettings is null) {
            throw new NullReferenceException("User settings object is null");
        }

        return startGameViewModel.ValidationRule(viewModel => viewModel.IpAddress,
                serverAddress => serverAddress != null && _validator.IsValidIpAddressOrUrl(serverAddress),
                _localeManager.GetStringByKey("LocalizedStrings.InvalidIpAddress", _userManager.UserSettings.Locale));
    }
}
