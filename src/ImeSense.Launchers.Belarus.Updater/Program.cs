using System.Diagnostics;
using System.Net.Http.Headers;

using ImeSense.Launchers.Belarus.Core;
using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.Core.Services;
using ImeSense.Launchers.Belarus.Core.Storage;

Console.Title = "Belarus Launcher Updater";

try {
    foreach (var process in Process.GetProcessesByName("SBLauncher")) {
        process.Kill();
    }

    Console.WriteLine($"Start update");
    var fileSavePath = Path.Combine(FileLocations.BaseDirectory, FileNamesStorage.SBLauncher);
    await UpdaterService.UpdaterAsync(UriStorage.LauncherUri, fileSavePath);
    Console.WriteLine($"Finish!");
    Launcher.Launch(fileSavePath)?.Start();
} catch (Exception ex) {
    Console.WriteLine(ex.Message);
    Console.WriteLine(ex.StackTrace);

    Console.ReadLine();
}
