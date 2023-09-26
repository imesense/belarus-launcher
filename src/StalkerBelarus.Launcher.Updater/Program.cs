namespace StalkerBelarus.Launcher.Updater;

internal class Program {
    private const string MutexName = "StalkerBelarus.Launcher";

    private static Mutex? _mutex;

    public static void Main() {
        var isMutexCreated = false;
        try {
            _mutex = new Mutex(initiallyOwned: false, MutexName, out isMutexCreated);
        } catch {
        }
        if (!isMutexCreated) {
            return;
        }

        try {
            new ProcessService().RunProcessAsync("SBLauncher.exe").Wait();
        } finally {
            _mutex?.Dispose();
        }
    }
}
