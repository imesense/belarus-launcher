using System.Diagnostics;
using System.Net.Http.Headers;

using ImeSense.Launchers.Belarus.Core;
using ImeSense.Launchers.Belarus.Core.Logger;
using ImeSense.Launchers.Belarus.Core.Manager;
using ImeSense.Launchers.Belarus.Core.Services;
using ImeSense.Launchers.Belarus.Core.Storage;

using Microsoft.Extensions.Logging;

using Serilog;

Console.Title = "Belarus Launcher Updater";

var pathLog = Path.Combine(DirectoryStorage.UserLogs, FileNameStorage.LauncherUpdaterLog);
using var factory = LoggerFactory.Create(builder => builder.AddSerilog(LogManager.CreateLoggerConsole(pathLog, true)));
var logger = factory.CreateLogger<Program>();
logger.LogInformation("Start Belarus Launcher Updater");

try {
    foreach (var process in Process.GetProcessesByName("SBLauncher")) {
        process.Kill();
    }

    logger.LogInformation($"Start update");
    var fileSavePath = Path.Combine(DirectoryStorage.Base, FileNameStorage.SBLauncher);

    using var httpClient = new HttpClient();
    httpClient.BaseAddress = UriStorage.LauncherApiUri;
    httpClient.DefaultRequestHeaders.Accept.Clear();
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
    httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

    var updaterService = new UpdaterService(factory.CreateLogger<UpdaterService>(),
        new GitHubApiService(factory.CreateLogger<GitHubApiService>(), httpClient, null), 
        new FileDownloadManager(factory.CreateLogger<FileDownloadManager>(), httpClient));
    await updaterService.UpdaterAsync(UriStorage.LauncherApiUri, fileSavePath);

    logger.LogInformation($"Finish!");
    Launcher.Launch(fileSavePath)?.Start();
} catch (Exception ex) {
    logger.LogInformation("{Message}", ex.Message);
    logger.LogInformation("{StackTrace}", ex.StackTrace);

    Console.ReadLine();
} finally {
    Log.CloseAndFlush();
}
