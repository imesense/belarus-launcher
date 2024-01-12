using System.Diagnostics;
using System.Reflection;

namespace ImeSense.Launchers.Belarus.Core.Helpers;

public static class ApplicationHelper {
    public static string GetAppVersion() {
        var entryAssembly = Assembly.GetEntryAssembly();
        var version = entryAssembly!.GetName().Version;
        return $"{version!.Major}.{version.Minor}";
    }

    public static string? GetCompanyName() {
        var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly()!.Location);
        return versionInfo.CompanyName;
    }
}
