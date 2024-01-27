using System.Diagnostics;

using ImeSense.Launchers.Belarus.Core.Storage;

namespace ImeSense.Launchers.Belarus.Core.Helpers;

public static class ProcessHelper {
    private static Process[] GetXrEngineProcesses() {
        return Process.GetProcessesByName(NameStorage.GameProcess);
    }

    public static void KillAllXrEngine() {
        foreach (var process in Process.GetProcessesByName(NameStorage.GameProcess)) {
            process.Kill();
        }
    }

    public static IEnumerable<Process> GetServerProcesses() {
        var processes = GetXrEngineProcesses()
            .Where(x => x.MainWindowTitle.Equals(NameStorage.TitleServerApp));
        foreach (var process in processes) {
            yield return process;
        }
    }

    public static void KillServers() {
        var processes = GetServerProcesses();
        var countServers = processes.Count();

        if (countServers <= 1) {
            return;
        }

        foreach (var process in processes.Take(1)) {
            if (process.MainWindowTitle.Equals(NameStorage.TitleServerApp)) {
                process.Kill();
            }
        }
    }
}
