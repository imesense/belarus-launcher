using System.Globalization;
using System.Text.Json;

using ImeSense.Launchers.Belarus.Core.Models;
using ImeSense.Launchers.Belarus.Core.Storage;
using ImeSense.Launchers.Belarus.Core.Validators;

using Microsoft.Extensions.Logging;

namespace ImeSense.Launchers.Belarus.Core.Manager;

public class UserManager(ILogger<UserManager>? logger,
    IAuthenticationValidator authenticationValidator,
    IStartGameValidator startGameValidator,
    ILauncherStorage launcherStorage)
{
    private readonly ILogger<UserManager>? _logger = logger;
    private readonly IAuthenticationValidator _authenticationValidator = authenticationValidator;
    private readonly IStartGameValidator _startGameValidator = startGameValidator;
    private readonly ILauncherStorage _launcherStorage = launcherStorage;

    public UserSettings? UserSettings { get; set; }

    public async Task LoadAsync()
    {
        if (!File.Exists(PathStorage.LauncherSetting)) {
            UserSettings = CreateDefaultUserSettings();
            return;
        }

        try {
            using var json = File.OpenRead(PathStorage.LauncherSetting);
            var user = await JsonSerializer.DeserializeAsync(json, SourceGenerationContext.Default.UserSettings);
            user ??= CreateDefaultUserSettings();

            if (!_startGameValidator.IsValidIpAddressOrUrl(user.IpAddress)) {
                user.IpAddress = string.Empty;
            }

            var isUsernameCorrect =
                _authenticationValidator.IsUsernameNotEmpty(user.Username) &&
                _authenticationValidator.IsUsernameCorrectLength(user.Username) &&
                _authenticationValidator.IsUsernameCorrectCharacters(user.Username);
            UserSettings = isUsernameCorrect
                ? user
                : CreateDefaultUserSettings();
        } catch {
            UserSettings = CreateDefaultUserSettings();
        }
    }

    public void Save()
    {
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

    public async Task SaveAsync()
    {
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

        await JsonSerializer.SerializeAsync(fileStream, UserSettings, typeof(UserSettings), SourceGenerationContext.Default);
    }

    private UserSettings CreateDefaultUserSettings()
    {
        var userSettings = new UserSettings();
        var systemCulture = CultureInfo.CurrentCulture;
        if (systemCulture.ThreeLetterISOLanguageName.Equals(_launcherStorage.Locales[0].Key)) {
            userSettings.Locale = _launcherStorage.Locales[0];
        } else {
            userSettings.Locale = _launcherStorage.Locales[1];
        }
        _logger?.LogInformation("Set locale: {locale}", userSettings.Locale.Title);

        return userSettings;
    }
}
