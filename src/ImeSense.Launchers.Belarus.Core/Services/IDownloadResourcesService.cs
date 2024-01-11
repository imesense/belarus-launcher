namespace ImeSense.Launchers.Belarus.Core.Services;

public interface IDownloadResourcesService {
    Task<IDictionary<string, Uri>?> GetFilesForDownloadAsync(IProgress<int> progress);
    Task DownloadAsync(string path, Uri url, IProgress<int> progress, CancellationTokenSource? tokenSource);
}
