using System.Text.RegularExpressions;

namespace ImeSense.Launchers.Belarus.Core.Validators;

public partial class AuthenticationValidator : IAuthenticationValidator {
    public bool IsUsernameNotEmpty(string username) =>
        !string.IsNullOrWhiteSpace(username) &&
        !string.IsNullOrEmpty(username);

    public bool IsUsernameCorrectLength(string username) =>
        username is { Length: <= 22 };

    public bool IsUsernameCorrectCharacters(string username) =>
        string.IsNullOrEmpty(username) ||
        (!string.IsNullOrWhiteSpace(username) &&
            UsernameCharactersRegex().IsMatch(username));

    [GeneratedRegex("^[a-zA-Z]+$")]
    private static partial Regex UsernameCharactersRegex();
}
