using System.Text.Json;

namespace ImeSense.Launchers.Belarus.Core.Helpers;

public static class FileDataHelper {
    public static async Task<T?> LoadDataAsync<T>(string filePath) {
        await using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read,
            FileShare.Read, 4096, FileOptions.Asynchronous);

        return await JsonSerializer.DeserializeAsync<T>(fileStream);
    }
}
