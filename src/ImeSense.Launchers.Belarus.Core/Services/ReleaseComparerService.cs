using System.Security.Cryptography;

using ImeSense.Launchers.Belarus.Core.Helpers;
using ImeSense.Launchers.Belarus.Core.Models;
using ImeSense.Launchers.Belarus.Core.Storage;

namespace ImeSense.Launchers.Belarus.Core.Services;

public class ReleaseComparerService : IReleaseComparerService<GitHubRelease>
{
    public async Task<bool> IsComparerAsync(GitHubRelease gitStorageRelease, CancellationToken cancellationToken = default)
    {
        var gitStorageReleaseStream = await SerializationHelper.SerializeToStreamAsync(gitStorageRelease, cancellationToken);
        var localRelease = await FileDataHelper.LoadDataAsync<GitHubRelease>(PathStorage.CurrentRelease, cancellationToken);
        var localReleaseStream = await SerializationHelper.SerializeToStreamAsync(localRelease, cancellationToken);

        using var md5 = MD5.Create();
        var currentReleaseHash = await md5.ComputeHashAsync(localReleaseStream, cancellationToken);
        localReleaseStream.Seek(0, SeekOrigin.Begin);
        var storageReleaseHash = await md5.ComputeHashAsync(gitStorageReleaseStream, cancellationToken);
        gitStorageReleaseStream.Seek(0, SeekOrigin.Begin);

        return currentReleaseHash.SequenceEqual(storageReleaseHash);
    }
}
