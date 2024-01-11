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
        var json = JsonSerializer.Serialize(settings, options);
        writer.Write(json);
    }

    public static UserSettings LoadSettings() {
        if (!File.Exists(FileLocations.UserSettingPath)) {
            return new UserSettings();
        }

        try {
            var json = File.ReadAllText(FileLocations.UserSettingPath);
            return JsonSerializer.Deserialize<UserSettings>(json)!;
        } catch {
            return new UserSettings();
        }
    }
}
