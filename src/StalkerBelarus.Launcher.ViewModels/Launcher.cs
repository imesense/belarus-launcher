using System.Diagnostics;

namespace StalkerBelarus.Launcher.ViewModels;

public static class Launcher {
    public static void Launch(string? path, string? workingDirectory = null,
        IList<string> arguments = null!) {
        if (string.IsNullOrEmpty(path)) {
            return;
        }
        if (!(File.Exists(path) && Path.GetExtension(path) != "exe")) {
            return;
        }

        var process = new Process {
            StartInfo = new ProcessStartInfo {
                FileName = path,
                Arguments = arguments != null
                    ? string.Join(" ", arguments)
                    : string.Empty,
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };
        process.Start();
    }
}
