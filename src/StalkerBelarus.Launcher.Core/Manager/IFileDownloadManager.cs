namespace StalkerBelarus.Launcher.Core.Manager;

public interface IFileDownloadManager {
    Task DownloadAsync(string url, string filePath, IProgress<int>? progress, CancellationToken token = default);
}
