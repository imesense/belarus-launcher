namespace StalkerBelarus.Launcher.Core.Models;

public class UserSettings {
    public string Username { get; set; } = string.Empty;

    public string IpAddress { get; set; } = string.Empty;

    public Locale Locale { get; set; } = new();

    public UserSettings() {
    }

    public UserSettings(string userName, string ipAddress, Locale locale) {
        Username = userName;
        IpAddress = ipAddress;
        Locale = locale;
    }
}
