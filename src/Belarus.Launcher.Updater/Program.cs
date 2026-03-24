using System.Diagnostics;

using Belarus.Launcher.Core;
using Belarus.Launcher.Core.Exceptions;
using Belarus.Launcher.Core.Http;
using Belarus.Launcher.Core.Logger;
using Belarus.Launcher.Core.Manager;
using Belarus.Launcher.Core.Services;
using Belarus.Launcher.Core.Storage;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Serilog;

const string title = "Belarus Launcher Updater";
Console.Title = title; // Only Windows system

var pathLog = Path.Combine(DirectoryStorage.LauncherLogs, FileNameStorage.LauncherUpdaterLog);
using var factory = LoggerFactory.Create(builder => builder.AddSerilog(LogManager.CreateLoggerConsole(pathLog)));
var logger = factory.CreateLogger<Program>();
GlobalExceptionHandler.Initialize(logger);
logger.LogInformation("{Info}", InformationPrinter.GetStartupInfo(title));

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var serviceApiToken = configuration["SecretsBelarus:GitHubToken"];
var uri = UriStorage.LauncherApiUri;
try
{
    foreach (var process in Process.GetProcessesByName("SBLauncher"))
    {
        process.Kill();
    }

    logger.LogInformation("Start update");
    var fileSavePath = Path.Combine(DirectoryStorage.CurrentDirectory, FileNameStorage.SBLauncherZip);

    using var httpClient = new HttpClient(HttpClientConfiguration.CreateHttpHandler());
    HttpClientConfiguration.Configure(httpClient, uri, serviceApiToken ?? throw new InvalidOperationException("Token is null"));

    var cancellationToken = new CancellationTokenSource();
    var updaterService = new UpdaterService(factory.CreateLogger<UpdaterService>(),
        new GitHubApiService(factory.CreateLogger<GitHubApiService>(), httpClient, null),
        new FileDownloadManager(factory.CreateLogger<FileDownloadManager>(), httpClient));
    await updaterService.UpdaterAsync(UriStorage.LauncherApiUri, fileSavePath, cancellationToken.Token);

    logger.LogInformation("Finish!");
    Launcher.Launch(Path.Combine(DirectoryStorage.CurrentDirectory, FileNameStorage.SBLauncher))?.Start();
}
catch (Exception ex)
{
    logger.LogInformation("{Message}", ex.Message);
    logger.LogInformation("{StackTrace}", ex.StackTrace);

    Console.ReadLine();
}
finally
{
    Log.CloseAndFlush();
}
