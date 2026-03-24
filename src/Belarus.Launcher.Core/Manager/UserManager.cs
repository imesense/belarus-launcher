using System.Globalization;
using System.Text.Json;

using Belarus.Launcher.Core.Models;
using Belarus.Launcher.Core.Storage;
using Belarus.Launcher.Core.Validators;

using Microsoft.Extensions.Logging;

namespace Belarus.Launcher.Core.Manager;

public class UserManager(ILogger<UserManager>? logger,
    IAuthenticationValidator authenticationValidator,
    IStartGameValidator startGameValidator,
    ILauncherStorage launcherStorage)
{
    public UserSettings? UserSettings { get; private set; }

    public static void MigratorSettings()
    {
        // Проверяем существование текущих настроек
        if (File.Exists(PathStorage.LauncherSetting))
        {
            return;
        }

        // Проверяем настройки версии 2.0 / 2.1
        if (File.Exists(PathStorage.V2LauncherSetting))
        {
            File.Move(PathStorage.V2LauncherSetting, PathStorage.LauncherSetting);
        }
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(PathStorage.LauncherSetting))
        {
            UserSettings = CreateDefaultUserSettings();
            return;
        }

        try
        {
            await using var json = File.OpenRead(PathStorage.LauncherSetting);
            var user = await JsonSerializer.DeserializeAsync(json, SourceGenerationContext.Default.UserSettings, cancellationToken);
            user ??= CreateDefaultUserSettings();

            if (!startGameValidator.IsValidIpAddressOrUrl(user.IpAddress))
            {
                user.IpAddress = string.Empty;
            }

            user.Locale ??= GetAutoLocale();

            var isUsernameCorrect =
                authenticationValidator.IsUsernameNotEmpty(user.Username) &&
                authenticationValidator.IsUsernameCorrectLength(user.Username) &&
                authenticationValidator.IsUsernameCorrectCharacters(user.Username);
            UserSettings = isUsernameCorrect
                ? user
                : CreateDefaultUserSettings();
        }
        catch
        {
            UserSettings = CreateDefaultUserSettings();
        }
    }

    public void Save()
    {
        if (UserSettings is null)
        {
            throw new NullReferenceException(nameof(UserSettings));
        }
        if (string.IsNullOrEmpty(UserSettings.Username))
        {
            throw new NullReferenceException("Username not specified");
        }

        if (!Directory.Exists(DirectoryStorage.AppData))
        {
            Directory.CreateDirectory(DirectoryStorage.AppData);
        }

        using var fileStream = new FileStream(PathStorage.LauncherSetting,
            FileMode.Create);
        using var writer = new StreamWriter(fileStream);

        var json = JsonSerializer.Serialize(UserSettings, typeof(UserSettings), SourceGenerationContext.Default);
        writer.Write(json);
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        if (UserSettings is null)
        {
            throw new NullReferenceException(nameof(UserSettings));
        }
        if (string.IsNullOrEmpty(UserSettings.Username))
        {
            throw new NullReferenceException("Username not specified");
        }

        if (!Directory.Exists(DirectoryStorage.AppData))
        {
            Directory.CreateDirectory(DirectoryStorage.AppData);
        }

        await using var fileStream = new FileStream(PathStorage.LauncherSetting,
            FileMode.Create);
        await using var writer = new StreamWriter(fileStream);

        await JsonSerializer.SerializeAsync(fileStream, UserSettings, typeof(UserSettings), SourceGenerationContext.Default, cancellationToken);
    }

    private UserSettings CreateDefaultUserSettings()
    {
        var userSettings = new UserSettings
        {
            Locale = GetAutoLocale()
        };
        logger?.LogInformation("Set locale: {locale}", userSettings.Locale.Title);
        return userSettings;
    }

    private Locale GetAutoLocale()
    {
        var systemCulture = CultureInfo.CurrentCulture;
        if (systemCulture.ThreeLetterISOLanguageName.Equals(launcherStorage.Locales[0].Key))
        {
            return launcherStorage.Locales[0];
        }

        return launcherStorage.Locales[1];
    }
}
