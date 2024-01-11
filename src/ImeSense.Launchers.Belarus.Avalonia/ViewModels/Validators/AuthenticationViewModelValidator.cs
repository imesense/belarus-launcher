using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.Core.Models;
using ImeSense.Launchers.Belarus.Core.Validators;

namespace ImeSense.Launchers.Belarus.Avalonia.ViewModels.Validators;

public sealed class AuthenticationViewModelValidator {
    private readonly IAuthenticationValidator _validator;
    private readonly ILocaleManager _localeManager;
    private readonly UserManager _userManager;

    public AuthenticationViewModelValidator(IAuthenticationValidator validator, ILocaleManager localeManager,
        UserManager userManager) {
        _validator = validator;
        _localeManager = localeManager;
        _userManager = userManager;
    }

    public ValidationHelper EnsureUsernameNotEmpty(AuthorizationViewModel authorizationViewModel) {
        if (_userManager is null) {
            throw new NullReferenceException("User manager object is null");
        }
        if (_userManager.UserSettings is null) {
            throw new NullReferenceException("User settings object is null");
        }
        if (_userManager.UserSettings.Locale is null) {
            throw new NullReferenceException("User settings locale object is null");
        }

        return authorizationViewModel.ValidationRule(viewModel => viewModel.Username,
            username => username != null && _validator.IsUsernameNotEmpty(username),
            _localeManager.GetStringByKey("LocalizedStrings.EnterNickName", _userManager.UserSettings.Locale.Key));
    }

    public ValidationHelper EnsureUsernameCorrectLength(AuthorizationViewModel authorizationViewModel) {
        if (_userManager is null) {
            throw new NullReferenceException("User manager object is null");
        }
        if (_userManager.UserSettings is null) {
            throw new NullReferenceException("User settings object is null");
        }
        if (_userManager.UserSettings.Locale is null) {
            throw new NullReferenceException("User settings locale object is null");
        }

        return authorizationViewModel.ValidationRule(viewModel => viewModel.Username,
            username => username != null && _validator.IsUsernameCorrectLength(username),
            _localeManager.GetStringByKey("LocalizedStrings.TooLongNickname", _userManager.UserSettings.Locale.Key));
    }

    public ValidationHelper EnsureUsernameCorrectCharacters(AuthorizationViewModel authorizationViewModel) {
        if (_userManager is null) {
            throw new NullReferenceException("User manager object is null");
        }
        if (_userManager.UserSettings is null) {
            throw new NullReferenceException("User settings object is null");
        }
        if (_userManager.UserSettings.Locale is null) {
            throw new NullReferenceException("User settings locale object is null");
        }

        return authorizationViewModel.ValidationRule(viewModel => viewModel.Username,
            username => username != null && _validator.IsUsernameCorrectCharacters(username),
            _localeManager.GetStringByKey("LocalizedStrings.InvalidCharacters", _userManager.UserSettings.Locale.Key));
    }
}
