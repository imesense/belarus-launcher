using System.Text.Json;

namespace StalkerBelarus.Launcher.Core.Helpers;

public static class SerializationHelper {
    public static async Task<MemoryStream> SerializeToStreamAsync<T>(T? obj)
    {
        var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, obj);
        stream.Position = 0;
        return stream;
    }
}
