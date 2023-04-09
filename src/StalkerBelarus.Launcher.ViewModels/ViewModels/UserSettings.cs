namespace StalkerBelarus.Launcher.ViewModels.Manager;

public class UserSettings : IUserSettings {
    public string UserName { get; set; } = string.Empty;
    public string IpAdress { get; set; } = "localhost";
}
