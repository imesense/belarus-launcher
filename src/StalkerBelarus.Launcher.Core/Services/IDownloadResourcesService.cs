namespace StalkerBelarus.Launcher.Core.Services;

public interface IDownloadResourcesService {
    Task DownloadsAsync(Progress<int> progress, CancellationTokenSource? tokenSource);
}
