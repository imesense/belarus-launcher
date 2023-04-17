using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace StalkerBelarus.Launcher.ViewModels.Manager;

public static class ConfigManager {
    public static readonly string Path =
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) +
        @"\sblauncher.json";

    public static void SaveSettings(UserSettings settings) {
        var options = new JsonSerializerOptions {
            AllowTrailingCommas = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };
        var json = JsonSerializer.Serialize(settings, options);
        if (!File.Exists(Path)) {
            File.Create(Path).Close();
        }
        File.WriteAllText(Path, json);
    }

    public static UserSettings LoadSettings() {
        if (File.Exists(Path)) {
            var json = File.ReadAllText(Path);
            return JsonSerializer.Deserialize<UserSettings>(json)!;
        }
        return new UserSettings();
    }
}
