using System.Globalization;
using System.Resources;

using ImeSense.Launchers.Belarus.Assets.ResXLocales;
using ImeSense.Launchers.Belarus.Core.Manager;

using Microsoft.Extensions.Logging;

using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace ImeSense.Launchers.Belarus.Manager;

public partial class LocalizationManager(ILogger<LocalizationManager>? logger) : ReactiveObject, IApplicationLocaleManager
{
    private readonly ResourceManager _resourceManager = Resources.ResourceManager;

    public string this[string key] => GetStringByKey(key);
    [Reactive] public partial string Locale { get; private set; }= CultureInfo.CurrentCulture.ThreeLetterISOLanguageName;

    public void SetLocale(string cultureCode)
    {
        Resources.Culture = new CultureInfo(cultureCode);
        Locale = Resources.Culture.ThreeLetterISOLanguageName;

        // Уведомляем об изменении всех свойств
        this.RaisePropertyChanged(string.Empty);
    }

    public string GetStringByKey(string key)
    {
        var text = _resourceManager.GetString(key, Resources.Culture);

        if (!string.IsNullOrEmpty(text))
        {
            return text;
        }

        logger?.LogError("Resource with key '{Key}' not found for locale '{Locale}'", key, Resources.Culture);
        return $"[{key}]";
    }
}
