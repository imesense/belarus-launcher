using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.Core.Models;
using ImeSense.Launchers.Belarus.Core.Validators;

namespace ImeSense.Launchers.Belarus.Avalonia.ViewModels.Validators;

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
        if (_userManager.UserSettings.Locale is null) {
            throw new NullReferenceException("User settings locale object is null");
        }

        return startGameViewModel.ValidationRule(viewModel => viewModel.IpAddress,
            serverAddress => serverAddress != null && _validator.IsIpAddressNotEmpty(serverAddress),
            _localeManager.GetStringByKey("LocalizedStrings.IpAddressNotEntered", _userManager.UserSettings.Locale.Key));
    }

    public ValidationHelper EnsureValidIpAddressOrUrl(StartGameViewModel startGameViewModel) {
        if (_userManager is null) {
            throw new NullReferenceException("User manager object is null");
        }
        if (_userManager.UserSettings is null) {
            throw new NullReferenceException("User settings object is null");
        }
        if (_userManager.UserSettings.Locale is null) {
            throw new NullReferenceException("User settings locale object is null");
        }

        return startGameViewModel.ValidationRule(viewModel => viewModel.IpAddress,
                serverAddress => serverAddress != null && _validator.IsValidIpAddressOrUrl(serverAddress),
                _localeManager.GetStringByKey("LocalizedStrings.InvalidIpAddress", _userManager.UserSettings.Locale.Key));
    }
}
