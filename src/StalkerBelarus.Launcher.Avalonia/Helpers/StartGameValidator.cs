using System.Text.RegularExpressions;

using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

using StalkerBelarus.Launcher.Avalonia.ViewModels;
using StalkerBelarus.Launcher.Core.Manager;

namespace StalkerBelarus.Launcher.Avalonia.Helpers;

public sealed partial class StartGameValidator {
    private readonly ILocaleManager _localeManager;

    public StartGameValidator(ILocaleManager localeManager) {
        _localeManager = localeManager;
    }
    
    public ValidationHelper EnsureIpAddressNotEmpty(StartGameViewModel startGameViewModel, string locale) =>
        startGameViewModel.ValidationRule(viewModel => viewModel.IpAddress,
            serverAddress => !string.IsNullOrWhiteSpace(serverAddress) && !string.IsNullOrEmpty(serverAddress),
            _localeManager.GetStringByKey("LocalizedStrings.IpAddressNotEntered", locale));
        public ValidationHelper EnsureValidIpAddressOrUrl(StartGameViewModel startGameViewModel, string locale) =>
            startGameViewModel.ValidationRule(viewModel => viewModel.IpAddress,
                serverAddress => string.IsNullOrEmpty(serverAddress) ||
                                 (!string.IsNullOrWhiteSpace(serverAddress) && MyRegex().IsMatch(serverAddress)),
                _localeManager.GetStringByKey("LocalizedStrings.InvalidIpAddress", locale));

    [GeneratedRegex(@"^(?:(?:https?|ftp):\/\/)?(?:www\.)?([a-zA-Z0-9-]+\.?)+[a-zA-Z]{2,}(?::\d+)?(?:\/[^\s]*)?$|^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(?::\d+)?$")]
    private static partial Regex MyRegex();
}
