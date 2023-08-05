namespace StalkerBelarus.Launcher.Core.Services;

public interface IDownloadResourcesService {
    Task<IDictionary<string, string>?> GetFilesForDownloadAsync(IProgress<int> progress);
    Task DownloadAsync(string path, string url, IProgress<int> progress, CancellationTokenSource? tokenSource);
}
