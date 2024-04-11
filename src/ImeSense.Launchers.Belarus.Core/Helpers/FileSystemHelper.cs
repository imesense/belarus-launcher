using System.Text.Json;

namespace ImeSense.Launchers.Belarus.Core.Helpers;

public static class FileSystemHelper
{
    public static async Task WriteReleaseAsync<T>(T obj, string path)
    {
        await using var fileStream = new FileStream(path, FileMode.Create);
        await using var writer = new StreamWriter(fileStream);

        await JsonSerializer.SerializeAsync(fileStream, obj, typeof(T), SourceGenerationContext.Default);
    }
}
