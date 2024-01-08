using System.Security.Cryptography;

using StalkerBelarus.Launcher.Core.Helpers;
using StalkerBelarus.Launcher.Core.Models;
using StalkerBelarus.Launcher.Core.Storage;

namespace StalkerBelarus.Launcher.Core.Services;

public class ReleaseComparerService : IReleaseComparerService<GitHubRelease>
{
    public async Task<bool> IsComparerAsync(GitHubRelease gitStorageRelease)
    {
        var gitStorageReleaseStream = await SerializationHelper.SerializeToStreamAsync(gitStorageRelease);
        var localRelease = await FileDataHelper.LoadDataAsync<GitHubRelease>(FileLocations.CurrentRelease);
        var localReleaseStream = await SerializationHelper.SerializeToStreamAsync(localRelease);

        using var md5 = MD5.Create();
        var currentReleaseHash = await md5.ComputeHashAsync(localReleaseStream);
        localReleaseStream.Seek(0, SeekOrigin.Begin);
        var storageReleaseHash = await md5.ComputeHashAsync(gitStorageReleaseStream);
        gitStorageReleaseStream.Seek(0, SeekOrigin.Begin);

        return currentReleaseHash.SequenceEqual(storageReleaseHash);
    }
}
