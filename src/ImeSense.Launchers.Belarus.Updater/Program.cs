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
    //foreach (var process in Process.GetProcessesByName("SBLauncher-x86")) {
    //    process.Kill();
    //}
    //foreach (var process in Process.GetProcessesByName("SBLauncher-x64")) {
    //    process.Kill();
    //}
    Console.WriteLine($"Start update");
    var filePath = Path.Combine(FileLocations.BaseDirectory, "SBLauncher.exe");

    if (File.Exists(filePath)) {
        File.Delete(filePath);
    }

    using var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri("https://api.github.com/repos/imesense/belarus-launcher/");
    httpClient.DefaultRequestHeaders.Accept.Clear();
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
    httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

    var gitHubService = new GitHubApiService(null, httpClient, null);

    var lastRelease = await gitHubService.GetLastReleaseAsync() 
        ?? throw new NullReferenceException("Latest release is null!");
    var sblauncher = lastRelease.Assets?.FirstOrDefault(x => x.Name.Equals("SBLauncher.exe"))
        ?? throw new NullReferenceException("SBLauncher asset is null!");

    if (sblauncher.BrowserDownloadUrl == null) {
        throw new NullReferenceException("Browser download url is null!");
    }
    var progress = new Progress<int>(percentage => {
        Console.WriteLine($"SBLauncher is {percentage}% downloaded");
    });

    var download = new FileDownloadManager(null, httpClient);
    await download.DownloadAsync(sblauncher.BrowserDownloadUrl, filePath, progress);

    Console.WriteLine($"Finish!");

    Launcher.Launch(filePath)?.Start();
} catch (Exception ex) {
    Console.WriteLine(ex.Message);
    Console.WriteLine(ex.StackTrace);

    Console.ReadLine();
}


