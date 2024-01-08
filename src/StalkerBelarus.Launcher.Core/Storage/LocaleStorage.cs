using StalkerBelarus.Launcher.Core.Models;

namespace StalkerBelarus.Launcher.Core.Storage;

public class LocaleStorage : ILocaleStorage {

    public IList<Locale> GetLocales() {
        return new List<Locale> {
            new() { Key = "ru", Title = "Русский", },
            // new() { Key = "be", Title = "Беларуская", },
            new() { Key = "en", Title = "English", },
        };
    }
}
