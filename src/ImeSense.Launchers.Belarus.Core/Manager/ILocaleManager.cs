namespace ImeSense.Launchers.Belarus.Core.Manager;

public interface ILocaleManager {
    void SetLocale(string locale);
    string GetStringByKey(string key, string locale);
}
