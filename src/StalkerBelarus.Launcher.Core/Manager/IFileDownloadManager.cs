namespace StalkerBelarus.Launcher.Core.Manager;

public interface IFileDownloadManager {
    Task DownloadAsync(Uri url, string filePath, IProgress<int>? progress, CancellationToken token = default);
}
