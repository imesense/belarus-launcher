using System.Text.RegularExpressions;

namespace ImeSense.Launchers.Belarus.Core.Validators;

public sealed partial class StartGameValidator : IStartGameValidator {

    public bool IsIpAddressNotEmpty(string serverAddress) =>
        !string.IsNullOrWhiteSpace(serverAddress) && !string.IsNullOrEmpty(serverAddress);

    public bool IsValidIpAddressOrUrl(string serverAddress) =>
        string.IsNullOrEmpty(serverAddress) ||
        (!string.IsNullOrWhiteSpace(serverAddress) &&
            IpAddressOrUrlRegex().IsMatch(serverAddress));

    [GeneratedRegex(@"^(?:(?:https?|ftp):\/\/)?(?:www\.)?([a-zA-Z0-9-]+\.?)+[a-zA-Z]{2,}(?::\d+)?(?:\/[^\s]*)?$|^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(?::\d+)?$")]
    private static partial Regex IpAddressOrUrlRegex();
}
