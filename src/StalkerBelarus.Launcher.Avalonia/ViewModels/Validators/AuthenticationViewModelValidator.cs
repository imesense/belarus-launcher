using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using StalkerBelarus.Launcher.Core.Manager;
using StalkerBelarus.Launcher.Core.Models;
using StalkerBelarus.Launcher.Core.Validators;

namespace StalkerBelarus.Launcher.Avalonia.ViewModels.Validators;

public sealed class AuthenticationViewModelValidator {
    private readonly IAuthenticationValidator _validator;
    private readonly ILocaleManager _localeManager;
    private readonly UserSettings _userSettings;

    public AuthenticationViewModelValidator(IAuthenticationValidator validator, ILocaleManager localeManager,
        UserSettings userSettings) {
        _validator = validator;
        _localeManager = localeManager;
        _userSettings = userSettings;
    }

    public ValidationHelper EnsureUsernameNotEmpty(AuthorizationViewModel authorizationViewModel) =>
        authorizationViewModel.ValidationRule(viewModel => viewModel.Username,
            username => username != null && _validator.IsUsernameNotEmpty(username),
            _localeManager.GetStringByKey("LocalizedStrings.EnterNickName", _userSettings.Locale));

    public ValidationHelper EnsureUsernameCorrectLength(AuthorizationViewModel authorizationViewModel) =>
        authorizationViewModel.ValidationRule(viewModel => viewModel.Username,
            username =>  username != null && _validator.IsUsernameCorrectLength(username),
            _localeManager.GetStringByKey("LocalizedStrings.TooLongNickname", _userSettings.Locale));

    public ValidationHelper EnsureUsernameCorrectCharacters(AuthorizationViewModel authorizationViewModel) =>
        authorizationViewModel.ValidationRule(viewModel => viewModel.Username,
            username => username != null && _validator.IsUsernameCorrectCharacters(username),
            _localeManager.GetStringByKey("LocalizedStrings.InvalidCharacters", _userSettings.Locale));
}
