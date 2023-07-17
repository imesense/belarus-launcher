namespace StalkerBelarus.Launcher.Core.Models;

public class UserSettings {
    public string Username { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;

    public UserSettings() {
        
    }

    public UserSettings(string userName, string ipAddress) {
        Username = userName;
        IpAddress = ipAddress;
    }
}
