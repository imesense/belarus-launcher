using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.Core.Storage;

using System.Net.Http.Headers;

namespace ImeSense.Launchers.Belarus.Core.Services;

// TODO: Decompose and deploy IoC
public class UpdaterService {
    public static async Task UpdaterAsync(Uri uri, string fileSavePath) {
        var appName = Path.GetFileNameWithoutExtension(fileSavePath);
        var fullAppName = Path.GetFileName(fileSavePath);

        using var httpClient = new HttpClient();
        httpClient.BaseAddress = uri;
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

        if (File.Exists(fileSavePath)) {
            File.Delete(fileSavePath);
        }
        File.Move(fileDownloadPath, fileSavePath);

        if (Directory.Exists(pathDownloadFolder)) {
            Directory.Delete(pathDownloadFolder);
        }
    }
}
