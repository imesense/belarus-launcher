using System.Diagnostics;
using System.Text.Json;

using ImeSense.Launchers.Belarus.Core.FileHashVerification;
using ImeSense.Launchers.Belarus.Core.Logger;
using ImeSense.Launchers.Belarus.Core.Models;
using ImeSense.Launchers.Belarus.Core.Storage;
using ImeSense.Launchers.Belarus.Core;

using Microsoft.Extensions.Logging;

using Serilog;

using static ImeSense.Launchers.Belarus.Core.Storage.DirectoryStorage;

IEnumerable<string> GetDirectories() => [Binaries, Resources, Patches];

var pathLog = Path.Combine(UserLogs, "CryptoHasherReport.log");
using var factory = LoggerFactory.Create(builder => builder.AddSerilog(LogManager.CreateLoggerConsole(pathLog, true)));
var logger = factory.CreateLogger<Md5HashProvider>();
logger.LogInformation("Start CryptoHasher application");

var hashing = new Md5HashProvider(logger);

try {
    var stopwatch = new Stopwatch();
    stopwatch.Start();

    var folders = GetDirectories();
    var gameResourceTasks = new List<Task<GameResource>>();

    foreach (var folderPath in folders) {
        var files = Directory.GetFiles(folderPath);
        foreach (var filePath in files) {
            gameResourceTasks.Add(AddGameResource(new FileInfo(filePath)));
        }
    }

    var gameResources = await Task.WhenAll(gameResourceTasks);
    stopwatch.Stop();
    logger.LogInformation("Hash calculation time: {time}", stopwatch.ElapsedMilliseconds);

    await using var fs = new FileStream(FileNameStorage.HashResources, FileMode.OpenOrCreate);
    await JsonSerializer.SerializeAsync(fs, gameResources, SourceGenerationContext.Default.GameResourceArray);
    fs.Close();

    logger.LogInformation("{json}", File.ReadAllText(FileNameStorage.HashResources));
} catch (Exception ex) {
    logger.LogInformation("{Message}", ex.Message);
    logger.LogInformation("{StackTrace}", ex.StackTrace);

    Console.ReadLine();
}

async Task<GameResource> AddGameResource(FileInfo fileInfo) {
    await using var stream = File.OpenRead(fileInfo.FullName);
    var hashFile = await hashing.CalculateHashAsync(stream);
    logger.LogInformation("File {FileName} {Length} Kb ({HashBytes})", fileInfo.Name, stream.Length / 1000.0f, hashFile);

    return new GameResource {
        Title = fileInfo.Name,
        Directory = fileInfo.Directory!.Name,
        Hash = await hashing.CalculateHashAsync(stream)
    };
}
