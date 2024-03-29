using System.Text.Json;

using ImeSense.Launchers.Belarus.Core.Models;
using ImeSense.Launchers.Belarus.Core.Storage;
using ImeSense.Launchers.Belarus.Core.Validators;

namespace ImeSense.Launchers.Belarus.Core.Manager;

public class UserManager {
    private readonly IAuthenticationValidator _authenticationValidator;
    private readonly IStartGameValidator _startGameValidator;

    public UserSettings? UserSettings { get; set; }

    public UserManager(IAuthenticationValidator authenticationValidator, IStartGameValidator startGameValidator) {
        _authenticationValidator = authenticationValidator;
        _startGameValidator = startGameValidator;

        Load();
    }

    private void Load() {
        if (!File.Exists(PathStorage.LauncherSetting)) {
            UserSettings = new UserSettings();

            return;
        }

        try {
            using var json = File.OpenRead(PathStorage.LauncherSetting);
            var user = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.UserSettings)!;

            if (!_startGameValidator.IsValidIpAddressOrUrl(user.IpAddress)) {
                user.IpAddress = string.Empty;
            }

            var isUsernameCorrect =
                _authenticationValidator.IsUsernameNotEmpty(user.Username) &&
                _authenticationValidator.IsUsernameCorrectLength(user.Username) &&
                _authenticationValidator.IsUsernameCorrectCharacters(user.Username);
            UserSettings = isUsernameCorrect
                ? user
                : new UserSettings();
        } catch {
            UserSettings = new UserSettings();
        }
    }

    public void Save() {
        if (UserSettings is null) {
            throw new ArgumentNullException(nameof(UserSettings));
        }
        if (string.IsNullOrEmpty(UserSettings.Username)) {
            throw new Exception("Username not specified");
        }

        if (!Directory.Exists(DirectoryStorage.User)) {
            Directory.CreateDirectory(DirectoryStorage.User);
        }

        using var fileStream = new FileStream(PathStorage.LauncherSetting,
            FileMode.Create);
        using var writer = new StreamWriter(fileStream);

        var json = JsonSerializer.Serialize(UserSettings, typeof(UserSettings), SourceGenerationContext.Default);
        writer.Write(json);
    }
}
