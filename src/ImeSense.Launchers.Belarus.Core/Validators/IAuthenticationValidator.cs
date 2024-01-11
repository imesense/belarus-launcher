namespace ImeSense.Launchers.Belarus.Core.Validators;

public interface IAuthenticationValidator {
    bool IsUsernameNotEmpty(string username);
    bool IsUsernameCorrectLength(string username);
    bool IsUsernameCorrectCharacters(string username);
}
