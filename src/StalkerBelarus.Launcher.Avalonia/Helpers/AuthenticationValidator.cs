using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using StalkerBelarus.Launcher.Avalonia.ViewModels;

namespace StalkerBelarus.Launcher.Avalonia.Helpers;

public sealed class AuthenticationValidator {
    public ValidationHelper EnsureUsernameNotEmpty(AuthorizationViewModel viewModel) =>
        viewModel.ValidationRule(viewModel => viewModel.Username,
            username => !string.IsNullOrEmpty(username) || !string.IsNullOrWhiteSpace(username),
            "Не введён ник");
}
