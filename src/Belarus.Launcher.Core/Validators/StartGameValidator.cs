using System.Text.RegularExpressions;

using Belarus.Launcher.Core.Storage;

namespace Belarus.Launcher.Core.Validators;

public sealed partial class StartGameValidator : IStartGameValidator
{
    public bool IsIpAddressNotEmpty(string serverAddress) => !string.IsNullOrWhiteSpace(serverAddress);

    public bool IsValidIpAddressOrUrl(string serverAddress) =>
        string.IsNullOrEmpty(serverAddress) ||
        (!string.IsNullOrWhiteSpace(serverAddress) &&
            IpAddressOrUrlRegex().IsMatch(serverAddress));

    [GeneratedRegex(RegexPatternStorage.IpAddressPattern)]
    private static partial Regex IpAddressOrUrlRegex();
}
