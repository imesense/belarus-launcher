namespace ImeSense.Launchers.Belarus.Core.Manager;

public interface IFileDownloadManager {
    Task DownloadAsync(Uri url, string filePath, IProgress<int>? progress, CancellationToken token = default);
}
