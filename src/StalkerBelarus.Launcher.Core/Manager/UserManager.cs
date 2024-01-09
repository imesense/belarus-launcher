using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

using StalkerBelarus.Launcher.Core.Models;
using StalkerBelarus.Launcher.Core.Storage;
using StalkerBelarus.Launcher.Core.Validators;

namespace StalkerBelarus.Launcher.Core.Manager;

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
        if (!File.Exists(FileLocations.UserSettingPath)) {
            UserSettings = new UserSettings();

            return;
        }

        try {
            var json = File.ReadAllText(FileLocations.UserSettingPath);
            var user = JsonSerializer.Deserialize<UserSettings>(json)!;
            if (!_startGameValidator.IsValidIpAddressOrUrl(user.IpAddress))
            {
                user.IpAddress = string.Empty;
            }

            var isUsernameCorrect = _authenticationValidator.IsUsernameNotEmpty(user.Username) &&
                                    _authenticationValidator.IsUsernameCorrectLength(user.Username) &&
                                    _authenticationValidator.IsUsernameCorrectCharacters(user.Username);
            UserSettings = isUsernameCorrect ? user : new UserSettings();
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
        if (!Directory.Exists(FileLocations.UserDirectory)) {
            Directory.CreateDirectory(FileLocations.UserDirectory);
        }
        using var fileStream = new FileStream(FileLocations.UserSettingPath,
            FileMode.Create);
        using var writer = new StreamWriter(fileStream);

        var options = new JsonSerializerOptions {
            AllowTrailingCommas = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };
        var json = JsonSerializer.Serialize(UserSettings, options);
        writer.Write(json);
    }
}
