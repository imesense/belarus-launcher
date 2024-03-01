using System.Diagnostics;
using System.Text.Json;

using ImeSense.Launchers.Belarus.Core.FileHashVerification;
using ImeSense.Launchers.Belarus.Core.Logger;
using ImeSense.Launchers.Belarus.Core.Models;
using ImeSense.Launchers.Belarus.Core.Storage;
using ImeSense.Launchers.Belarus.Core;

using Microsoft.Extensions.Logging;

using Serilog;

Console.Title = "Belarus CryptoHasher";

IEnumerable<string> GetDirectories() => [DirectoryStorage.Binaries, DirectoryStorage.Resources, DirectoryStorage.Patches];

var pathLog = Path.Combine(DirectoryStorage.UserLogs, FileNameStorage.CryptoHasherLog);
using var factory = LoggerFactory.Create(builder => builder.AddSerilog(LogManager.CreateLoggerConsole(pathLog, true)));
var logger = factory.CreateLogger<Program>();
logger.LogInformation("Start CryptoHasher application");

var hashing = new Md5HashProvider(factory.CreateLogger<Md5HashProvider>());

try {
    var gameResourceTasks = new List<Task<GameResource>>();
    var gameResources = new List<GameResource>();
    
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    foreach (var folderPath in GetDirectories()) {
        var dir = new DirectoryInfo(folderPath);
        foreach (var file in dir.GetFiles()) {
            if (file.Length > 100000000) {
                gameResourceTasks.Add(AddGameResourceAsync(file));
            } else {
                gameResources.Add(AddGameResource(file));
            }
        }
    }

    gameResources.AddRange(await Task.WhenAll(gameResourceTasks));
    stopwatch.Stop();
    logger.LogInformation("Hash calculation time: {time}mc", stopwatch.ElapsedMilliseconds);

    await using var fs = new FileStream(FileNameStorage.HashResources, FileMode.CreateNew);
    await JsonSerializer.SerializeAsync(fs, gameResources.ToArray(), SourceGenerationContext.Default.GameResourceArray);
    fs.Close();

    logger.LogInformation("{json}", File.ReadAllText(FileNameStorage.HashResources));
} catch (Exception ex) {
    logger.LogInformation("{Message}", ex.Message);
    logger.LogInformation("{StackTrace}", ex.StackTrace);

    Console.ReadLine();
} finally {
    Log.CloseAndFlush();
}

async Task<GameResource> AddGameResourceAsync(FileInfo fileInfo) {
    var stopwatch = new Stopwatch();
    stopwatch.Start();

    await using var stream = File.OpenRead(fileInfo.FullName);
    var hashFile = await hashing.CalculateHashAsync(stream);
    logger.LogInformation("File {FileName} {Length}Kb Time:{time}ms ({HashBytes}) ", 
        fileInfo.Name, stream.Length / 1000, stopwatch.ElapsedMilliseconds, hashFile);
    stopwatch.Stop();

    return new GameResource {
        Title = fileInfo.Name,
        Directory = fileInfo.Directory!.Name,
        Hash = hashFile
    };
}

GameResource AddGameResource(FileInfo fileInfo) {
    var stopwatch = new Stopwatch();
    stopwatch.Start();

    using var stream = File.OpenRead(fileInfo.FullName);
    var hashFile = hashing.CalculateHash(stream);
    logger.LogInformation("File {FileName} {Length}Kb Time:{time}ms ({HashBytes}) ", 
        fileInfo.Name, stream.Length / 1000, stopwatch.ElapsedMilliseconds, hashFile);
    stopwatch.Stop();

    return new GameResource {
        Title = fileInfo.Name,
        Directory = fileInfo.Directory!.Name,
        Hash = hashFile
    };
}
