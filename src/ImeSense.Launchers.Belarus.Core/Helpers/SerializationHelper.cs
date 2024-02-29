using System.Text.Json;

namespace ImeSense.Launchers.Belarus.Core.Helpers;

public static class SerializationHelper {
    public static async Task<MemoryStream> SerializeToStreamAsync<T>(T? obj) {
        var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, obj, typeof(T), SourceGenerationContext.Default);
        stream.Position = 0;
        return stream;
    }
}
