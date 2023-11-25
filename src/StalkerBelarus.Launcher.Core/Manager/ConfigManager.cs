using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

using StalkerBelarus.Launcher.Core.Models;
using StalkerBelarus.Launcher.Core.Storage;

namespace StalkerBelarus.Launcher.Core.Manager;

public class UserManager {
    public UserSettings? UserSettings { get; set; }

    public UserManager() {
        Load();
    }

    public void Load() {
        if (!File.Exists(FileLocations.UserSettingPath)) {
            UserSettings = new UserSettings();
        }

        try {
            var json = File.ReadAllText(FileLocations.UserSettingPath);
            UserSettings = JsonSerializer.Deserialize<UserSettings>(json)!;
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
