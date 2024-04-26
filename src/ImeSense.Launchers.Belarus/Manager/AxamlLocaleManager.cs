using Avalonia.Markup.Xaml.Styling;

using ImeSense.Launchers.Belarus.Core.Manager;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ImeSense.Launchers.Belarus.Manager;

public class AxamlLocaleManager(ILogger<AxamlLocaleManager> logger) : ReactiveObject, IApplicationLocaleManager
{
    private readonly ILogger<AxamlLocaleManager> _logger = logger;
    private ResourceInclude? _resources;

    [Reactive] public string Locale { get; private set; } = string.Empty;

    public void SetLocale(string locale)
    {
        App.Current?.Resources.Clear();
        LoadLocalizedResources(locale);
        Locale = locale;
    }

    private void LoadLocalizedResources(string locale)
    {
        try {
            _resources = new ResourceInclude(new Uri("avares://SBLauncher/Assets/Locales/")) {
                Source = new Uri($"avares://SBLauncher/Assets/Locales/{locale}.axaml")
            };
            App.Current?.Resources.MergedDictionaries.Add(_resources);
        } catch (Exception ex) {
            _logger.LogError(ex, "Failed to load localized resources for locale: {Locale}", locale);
            throw;
        }
    }

    public string GetStringByKey(string key)
    {
        if (_resources is null) {
            _logger.LogError("Resource include is not initialized");
            return string.Empty;
        }

        var resources = _resources.Loaded;
        if (resources.TryGetValue(key, out var value)) {
            return (string) value!;
        } else {
            _logger.LogError("Resource with key '{Key}' not found for locale '{Locale}'", key, Locale);
            return string.Empty;
        }
    }
}
