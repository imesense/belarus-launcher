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

    using var httpClient = new HttpClient();
    httpClient.BaseAddress = UriStorage.LauncherUri;
    httpClient.DefaultRequestHeaders.Accept.Clear();
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
    httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

    var updaterService = new UpdaterService( 
        new GitHubApiService(null, httpClient, null), 
        new FileDownloadManager(null, httpClient));
    await updaterService.UpdaterAsync(UriStorage.LauncherUri, fileSavePath);

    Console.WriteLine($"Finish!");
    Launcher.Launch(fileSavePath)?.Start();
} catch (Exception ex) {
    Console.WriteLine(ex.Message);
    Console.WriteLine(ex.StackTrace);

    Console.ReadLine();
}
