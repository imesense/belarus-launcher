namespace ImeSense.Launchers.Belarus.Core.Services;

public interface IUpdaterService {
    Task UpdaterAsync(Uri uri, string fileSavePath);
}
