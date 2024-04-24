using Avalonia.Controls;
using Avalonia.Markup.Xaml.Styling;

using ImeSense.Launchers.Belarus.Core.Manager;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ImeSense.Launchers.Belarus.Avalonia.Manager;

public class AxamlLocaleManager : ReactiveObject, IApplicationLocaleManager
{
    [Reactive]
    public string Locale { get; private set; } = string.Empty;

    public void SetLocale(string locale)
    {
        Locale = locale;

        App.Current?.Resources.Clear();
        var resource = new ResourceInclude(new Uri("avares://SBLauncher/Assets/Locales/")) {
            Source = new Uri($"avares://SBLauncher/Assets/Locales/{Locale}.axaml"),
        };
        App.Current?.Resources.MergedDictionaries.Add(resource);
    }

    public string GetStringByKey(string key)
    {
        var resources = new ResourceInclude(new Uri("avares://SBLauncher/Assets/Locales/")) {
            Source = new Uri($"avares://SBLauncher/Assets/Locales/{Locale}.axaml"),
        };
        var control = new Control {
            Resources = resources.Loaded,
        };
        if (control.TryFindResource(key, out var value)) {
            return (string) value!;
        }
        return string.Empty;
    }
}
