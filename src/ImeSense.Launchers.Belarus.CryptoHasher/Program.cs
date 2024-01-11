using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

using ImeSense.Launchers.Belarus.Core.FileHashVerification;
using ImeSense.Launchers.Belarus.Core.Models;
using ImeSense.Launchers.Belarus.Core.Storage;

using static ImeSense.Launchers.Belarus.Core.Storage.FileLocations;

var hashing = new Md5HashProvider(null);
var folders = GetDirectories();
var gameResources = new List<GameResource>();

foreach (var folderPath in folders)  {
    var files = Directory.GetFiles(folderPath);
    foreach (var filePath in files)
    {
        var fileInfo = new FileInfo(filePath);

        await using var stream = File.OpenRead(filePath);
        var file = new GameResource { 
            Title = fileInfo.Name,
            Directory = fileInfo.Directory!.Name,
            Hash = await hashing.CalculateHashAsync(stream)
        };
        gameResources.Add(file);
    }
}

var options = new JsonSerializerOptions {
    AllowTrailingCommas = true,
    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
    WriteIndented = true
};
await using var fs = new FileStream(FileNamesStorage.HashResources, FileMode.OpenOrCreate);
await JsonSerializer.SerializeAsync(fs, gameResources, options);
using var reader = new StreamReader(fs);

Console.WriteLine(File.ReadAllText(FileNamesStorage.HashResources));
return;

IEnumerable<string> GetDirectories() {
    return new[] { BinariesDirectory, ResourcesDirectory, PatchesDirectory };
}

