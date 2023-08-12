namespace StalkerBelarus.Launcher.Avalonia.Manager;

public interface ILocaleManager {
    void SetLocale(string locale);
    string GetStringByKey(string key, string locale);
}
