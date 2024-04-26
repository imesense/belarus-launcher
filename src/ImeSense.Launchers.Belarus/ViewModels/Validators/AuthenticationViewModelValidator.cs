using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.Core.Validators;

using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace ImeSense.Launchers.Belarus.ViewModels.Validators;

public sealed class AuthenticationViewModelValidator(IAuthenticationValidator validator, IApplicationLocaleManager localeManager)
{
    private readonly IAuthenticationValidator _validator = validator;
    private readonly IApplicationLocaleManager _localeManager = localeManager;

    public ValidationHelper EnsureUsernameNotEmpty(AuthorizationViewModel authorizationViewModel)
    {
        return authorizationViewModel.ValidationRule(viewModel => viewModel.Username,
            username => username != null && _validator.IsUsernameNotEmpty(username),
            _localeManager.GetStringByKey("LocalizedStrings.EnterNickName"));
    }

    public ValidationHelper EnsureUsernameCorrectLength(AuthorizationViewModel authorizationViewModel)
    {
        return authorizationViewModel.ValidationRule(viewModel => viewModel.Username,
            username => username != null && _validator.IsUsernameCorrectLength(username),
            _localeManager.GetStringByKey("LocalizedStrings.TooLongNickname"));
    }

    public ValidationHelper EnsureUsernameCorrectCharacters(AuthorizationViewModel authorizationViewModel)
    {
        return authorizationViewModel.ValidationRule(viewModel => viewModel.Username,
            username => username != null && _validator.IsUsernameCorrectCharacters(username),
            _localeManager.GetStringByKey("LocalizedStrings.InvalidCharacters"));
    }
}
