using System.Diagnostics;

namespace ImeSense.Launchers.Belarus.Core;

public static class Launcher {
    public static Process? Launch(string? path, string? workingDirectory = null,
        IList<string> arguments = null!) {
        if (string.IsNullOrEmpty(path)) {
            return default;
        }
        if (!(File.Exists(path) && Path.GetExtension(path) != "exe")) {
            return default;
        }

        var process = new Process {
            StartInfo = new ProcessStartInfo {
                FileName = path,
                Arguments = arguments != null
                    ? string.Join(" ", arguments)
                    : string.Empty,
                WorkingDirectory = workingDirectory,
                UseShellExecute = true,
                CreateNoWindow = true,
                Verb = "runas"
            },
            EnableRaisingEvents = true
        };

        return process;
    }
}
