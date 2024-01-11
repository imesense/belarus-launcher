using System.Diagnostics;

namespace ImeSense.Launchers.Belarus.Reloader;

public class ProcessService {
    private const int RestartCount = 1;

    private string _name = string.Empty;
    private string _path = string.Empty;

    private int _restartCount;

    public async Task RunProcessAsync(string name) {
        _name = name;

        var location = Process.GetCurrentProcess().MainModule?.FileName;
        if (location is null) {
            return;
        }

        var workingDirectory = AppDomain.CurrentDomain.BaseDirectory;
        _path = Path.Combine(workingDirectory, _name);

        var startInfo = new ProcessStartInfo(_path) {
            WorkingDirectory = workingDirectory,
            UseShellExecute = true,
        };
        var process = Process.Start(startInfo);
        if (process is null) {
            return;
        }

        await WaitForExitAsync(process).ConfigureAwait(false);
    }

    private async Task WaitForExitAsync(Process process) {
        process.EnableRaisingEvents = true;

        await TaskFromEventAsync(h => process.Exited += h, h => process.Exited -= h, CancellationToken.None).ConfigureAwait(false);
        await ProcessExitedHandlerAsync(process).ConfigureAwait(false);
    }

    private async Task ProcessExitedHandlerAsync(Process process) {
        if (process.ExitCode == 0 || _restartCount == RestartCount) {
            Environment.Exit(0);
            return;
        }

        var workingDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var startInfo = new ProcessStartInfo(_path) {
            WorkingDirectory = workingDirectory,
            UseShellExecute = true,
        };
        var restartedProcess = Process.Start(startInfo);
        if (restartedProcess is null) {
            return;
        }
        _restartCount++;

        await WaitForExitAsync(restartedProcess).ConfigureAwait(false);
    }

    private static async Task<EventArgs> TaskFromEventAsync(Action<EventHandler> registerEvent, Action<EventHandler> unregisterEvent, CancellationToken token) {
        var source = new TaskCompletionSource<EventArgs>();
        registerEvent(Handler);

        try {
            using (token.Register(source.SetCanceled)) {
                return await source.Task.ConfigureAwait(continueOnCapturedContext: false);
            }
        } finally {
            unregisterEvent(Handler);
        }

        void Handler(object? sender, EventArgs args) {
            source.TrySetResult(args);
        }
    }
}
