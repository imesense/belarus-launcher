using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

using ImeSense.Launchers.Belarus.Core.Models;
using ImeSense.Launchers.Belarus.Core.Storage;

namespace ImeSense.Launchers.Belarus.Legacy.Manager;

public static class ConfigManager {
    public static void SaveSettings(UserSettings settings) {
        if (settings is null) {
            throw new ArgumentNullException(nameof(settings));
        }
        if (string.IsNullOrEmpty(settings.Username)) {
            throw new Exception("Username not specified");
        }

        if (!Directory.Exists(DirectoryStorage.User)) {
            Directory.CreateDirectory(DirectoryStorage.User);
        }

        using var fileStream = new FileStream(PathStorage.LauncherSetting,
            FileMode.Create);
        using var writer = new StreamWriter(fileStream);

        var options = new JsonSerializerOptions {
            AllowTrailingCommas = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };
        var json = JsonSerializer.Serialize(settings, options);
        writer.Write(json);
    }

    public static UserSettings LoadSettings() {
        if (!File.Exists(PathStorage.LauncherSetting)) {
            return new UserSettings();
        }

        try {
            var json = File.ReadAllText(PathStorage.LauncherSetting);
            return JsonSerializer.Deserialize<UserSettings>(json)!;
        } catch {
            return new UserSettings();
        }
    }
}
