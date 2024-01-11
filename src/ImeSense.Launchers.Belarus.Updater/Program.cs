using System.Diagnostics;
using System.Net.Http.Headers;

using ImeSense.Launchers.Belarus.Core;
using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.Core.Services;
using ImeSense.Launchers.Belarus.Core.Storage;

Console.Title = "Belarus Launcher Updater";

try {
    var fullAppName = "SBLauncher.exe";
    var appName = Path.GetFileNameWithoutExtension(fullAppName);

    foreach (var process in Process.GetProcessesByName(appName)) {
        process.Kill();
    }

    Console.WriteLine($"Start update");

    using var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri("https://api.github.com/repos/imesense/belarus-launcher/");
    httpClient.DefaultRequestHeaders.Accept.Clear();
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
    httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

    var gitHubService = new GitHubApiService(null, httpClient, null);

    var lastRelease = await gitHubService.GetLastReleaseAsync() 
        ?? throw new NullReferenceException("Latest release is null!");
    var sblauncher = lastRelease.Assets?.FirstOrDefault(x => x.Name.Equals(fullAppName))
        ?? throw new NullReferenceException($"{appName} asset is null!");

    if (sblauncher.BrowserDownloadUrl == null) {
        throw new NullReferenceException("Browser download url is null!");
    }
    var progress = new Progress<int>(percentage => {
        Console.WriteLine($"{appName} is {percentage}% downloaded");
    });

    var pathDownloadFolder = Path.Combine(FileLocations.BaseDirectory, "temp");
    var fileDownloadPath = Path.Combine(pathDownloadFolder, fullAppName);
    
    if (!Directory.Exists(pathDownloadFolder)) {
        Directory.CreateDirectory(pathDownloadFolder);
    }

    var download = new FileDownloadManager(null, httpClient);
    await download.DownloadAsync(sblauncher.BrowserDownloadUrl, fileDownloadPath, progress);

    var filePath = Path.Combine(FileLocations.BaseDirectory, fullAppName);

    if (File.Exists(filePath)) {
        File.Delete(filePath);
    }
    File.Move(fileDownloadPath, filePath);

    if (Directory.Exists(pathDownloadFolder)) {
        Directory.Delete(pathDownloadFolder);
    }

    Console.WriteLine($"Finish!");

    Launcher.Launch(filePath)?.Start();
} catch (Exception ex) {
    Console.WriteLine(ex.Message);
    Console.WriteLine(ex.StackTrace);

    Console.ReadLine();
}
