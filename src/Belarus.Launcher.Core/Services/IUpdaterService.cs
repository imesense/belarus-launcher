namespace Belarus.Launcher.Core.Services;

public interface IUpdaterService
{
    Task UpdaterAsync(Uri uri, string fileSavePath, CancellationToken cancellationToken = default);
}
