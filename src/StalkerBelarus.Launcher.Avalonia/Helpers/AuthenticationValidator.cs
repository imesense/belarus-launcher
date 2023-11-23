using System.Text.RegularExpressions;

using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using StalkerBelarus.Launcher.Avalonia.ViewModels;
using StalkerBelarus.Launcher.Core.Manager;

namespace StalkerBelarus.Launcher.Avalonia.Helpers;

public sealed partial class AuthenticationValidator {
    private readonly ILocaleManager _localeManager;

    public AuthenticationValidator(ILocaleManager localeManager) {
        _localeManager = localeManager;
    }

    public ValidationHelper EnsureUsernameNotEmpty(AuthorizationViewModel authorizationViewModel, string locale) =>
        authorizationViewModel.ValidationRule(viewModel => viewModel.Username,
            username => !string.IsNullOrWhiteSpace(username) && !string.IsNullOrEmpty(username),
            _localeManager.GetStringByKey("LocalizedStrings.EnterNickName", locale));

    public ValidationHelper EnsureUsernameCorrectLength(AuthorizationViewModel authorizationViewModel, string locale) =>
        authorizationViewModel.ValidationRule(viewModel => viewModel.Username,
            username =>  username is { Length: <= 22 },
            _localeManager.GetStringByKey("LocalizedStrings.TooLongNickname", locale));

    public ValidationHelper EnsureUsernameCorrectCharacters(AuthorizationViewModel authorizationViewModel, string locale) =>
        authorizationViewModel.ValidationRule(viewModel => viewModel.Username,
            username => string.IsNullOrEmpty(username) ||
                        (!string.IsNullOrWhiteSpace(username) && UsernameCharactersRegex().IsMatch(username)),
            _localeManager.GetStringByKey("LocalizedStrings.InvalidCharacters", locale));

    [GeneratedRegex("^[a-zA-Z]+$")]
    private static partial Regex UsernameCharactersRegex();
}
