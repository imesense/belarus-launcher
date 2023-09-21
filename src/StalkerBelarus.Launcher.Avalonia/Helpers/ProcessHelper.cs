using System.Diagnostics;

namespace StalkerBelarus.Launcher.Avalonia.Helpers; 

public static class ProcessHelper {
    private static Process[] GetXrEngineProcesses() {
        return Process.GetProcessesByName("xrEngine");
    }
    
    public static void KillAllXrEngine() {
        foreach (var process in Process.GetProcessesByName("xrEngine")) {
            process.Kill();
        }
    }
    
    public static IEnumerable<Process> GetServerProcesses() {
        var processes = GetXrEngineProcesses()
            .Where(x => x.MainWindowTitle.Equals("S.T.A.L.K.E.R.: Belarus Server"));
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
            if (process.MainWindowTitle.Equals("S.T.A.L.K.E.R.: Belarus Server")) {
                process.Kill();
            }
        }
    }
}
