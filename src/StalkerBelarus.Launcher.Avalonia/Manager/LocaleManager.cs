using Avalonia.Markup.Xaml.Styling;

namespace StalkerBelarus.Launcher.Avalonia.Manager;

public class LocaleManager : ILocaleManager {
    public void SetLocale(string locale) {
        App.Current?.Resources.Clear();
        var resource = new ResourceInclude(new Uri("avares://SBLauncher/Assets/Locales/")) {
            Source = new Uri($"avares://SBLauncher/Assets/Locales/{locale}.axaml"),
        };
        App.Current?.Resources.MergedDictionaries.Add(resource);
    }
}
