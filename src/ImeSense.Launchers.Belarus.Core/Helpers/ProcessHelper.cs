using System.Diagnostics;

using ImeSense.Launchers.Belarus.Core.Storage;

namespace ImeSense.Launchers.Belarus.Core.Helpers;

public static class ProcessHelper {
    private static Process[] GetXrEngineProcesses() {
        return Process.GetProcessesByName(NamesStorage.GameProcess);
    }

    public static void KillAllXrEngine() {
        foreach (var process in Process.GetProcessesByName(NamesStorage.GameProcess)) {
            process.Kill();
        }
    }

    public static IEnumerable<Process> GetServerProcesses() {
        var processes = GetXrEngineProcesses()
            .Where(x => x.MainWindowTitle.Equals(NamesStorage.TitleServerApp));
        foreach (var process in processes) {
            yield return process;
        }
    }

    public static void KillServers() {
        var processes = GetServerProcesses();
        var countServers = processes.Count();

        if (countServers <= 1)
            return;

        foreach (var process in processes.Take(1)) {
            if (process.MainWindowTitle.Equals(NamesStorage.TitleServerApp)) {
                process.Kill();
            }
        }
    }
}
