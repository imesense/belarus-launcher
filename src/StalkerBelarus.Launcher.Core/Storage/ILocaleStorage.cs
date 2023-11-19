using StalkerBelarus.Launcher.Core.Models;

namespace StalkerBelarus.Launcher.Core.Storage;

public interface ILocaleStorage {

    IList<Locale> GetLocales();
}
