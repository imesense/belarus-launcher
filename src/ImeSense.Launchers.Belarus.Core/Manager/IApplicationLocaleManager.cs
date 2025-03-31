namespace ImeSense.Launchers.Belarus.Core.Manager;

public interface IApplicationLocaleManager
{
    void SetLocale(string locale);
    string Locale { get; }
    string GetStringByKey(string key);
    string this[string key] { get; }
}
