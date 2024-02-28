using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

using ImeSense.Launchers.Belarus.Core.FileHashVerification;
using ImeSense.Launchers.Belarus.Core.Models;
using ImeSense.Launchers.Belarus.Core.Storage;

using static ImeSense.Launchers.Belarus.Core.Storage.DirectoryStorage;

IEnumerable<string> GetDirectories() {
    return new[] { Binaries, Resources, Patches };
}
var hashing = new Md5HashProvider(null);

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
    Console.WriteLine($"Hash calculation time: {stopwatch.ElapsedMilliseconds}");

    var options = new JsonSerializerOptions {
        AllowTrailingCommas = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        WriteIndented = true
    };
    await using var fs = new FileStream(FileNameStorage.HashResources, FileMode.OpenOrCreate);
    await JsonSerializer.SerializeAsync(fs, gameResources, options);
    fs.Close();

    Console.WriteLine(File.ReadAllText(FileNameStorage.HashResources));
} catch (Exception ex) {
    Console.WriteLine(ex.Message);
    Console.WriteLine(ex.StackTrace);

    Console.ReadLine();
}

async Task<GameResource> AddGameResource(FileInfo fileInfo) {
    Console.WriteLine($"Calculating hash file: {fileInfo.Name} {fileInfo.Length / 1000.0f} Kb");
    await using var stream = File.OpenRead(fileInfo.FullName);
    return new GameResource {
        Title = fileInfo.Name,
        Directory = fileInfo.Directory!.Name,
        Hash = await hashing.CalculateHashAsync(stream)
    };
}
