using Avalonia.Markup.Xaml.Styling;

using Belarus.Launcher.Core.Manager;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace Belarus.Launcher.Manager;

public partial class AxamlLocaleManager(ILogger<AxamlLocaleManager> logger) : ReactiveObject, IApplicationLocaleManager
{
    private ResourceInclude? _resources;

    [Reactive] public partial string Locale { get; private set; } = string.Empty;

    public void SetLocale(string locale)
    {
        App.Current?.Resources.Clear();
        LoadLocalizedResources(locale);
        Locale = locale;
    }

    private void LoadLocalizedResources(string locale)
    {
        try
        {
            _resources = new ResourceInclude(new Uri("avares://Belarus.Launcher/Assets/Locales/"))
            {
                Source = new Uri($"avares://Belarus.Launcher/Assets/Locales/{locale}.axaml")
            };
            App.Current?.Resources.MergedDictionaries.Add(_resources);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load localized resources for locale: {Locale}", locale);
            throw;
        }
    }

    public string GetStringByKey(string key)
    {
        if (_resources is null)
        {
            logger.LogError("Resource include is not initialized");
            return string.Empty;
        }

        var resources = _resources.Loaded;
        if (resources.TryGetValue(key, out var value))
        {
            return (string) value!;
        }
        else
        {
            logger.LogError("Resource with key '{Key}' not found for locale '{Locale}'", key, Locale);
            return string.Empty;
        }
    }

    public string this[string key] => GetStringByKey(key);
}
