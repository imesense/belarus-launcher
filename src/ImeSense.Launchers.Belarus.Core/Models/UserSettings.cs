using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ImeSense.Launchers.Belarus.Core.Models;

public class UserSettings : ReactiveObject
{
    public string Username { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;

    [Reactive]
    public Locale? Locale { get; set; }

    public UserSettings()
    {
    }

    public UserSettings(string userName, string ipAddress, Locale locale)
    {
        Username = userName;
        IpAddress = ipAddress;
        Locale = locale;
    }
}
