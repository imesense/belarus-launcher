using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.Core.Validators;

using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace ImeSense.Launchers.Belarus.ViewModels.Validators;

public sealed class StartGameViewModelValidator(IStartGameValidator validator, IApplicationLocaleManager localeManager)
{
    private readonly IStartGameValidator _validator = validator;
    private readonly IApplicationLocaleManager _localeManager = localeManager;

    public ValidationHelper EnsureIpAddressNotEmpty(StartGameViewModel startGameViewModel)
    {

        return startGameViewModel.ValidationRule(viewModel => viewModel.IpAddress,
            serverAddress => serverAddress != null && _validator.IsIpAddressNotEmpty(serverAddress),
            _localeManager.GetStringByKey("LocalizedStrings.IpAddressNotEntered"));
    }

    public ValidationHelper EnsureValidIpAddressOrUrl(StartGameViewModel startGameViewModel)
    {
        return startGameViewModel.ValidationRule(viewModel => viewModel.IpAddress,
                serverAddress => serverAddress != null && _validator.IsValidIpAddressOrUrl(serverAddress),
                _localeManager.GetStringByKey("LocalizedStrings.InvalidIpAddress"));
    }
}
