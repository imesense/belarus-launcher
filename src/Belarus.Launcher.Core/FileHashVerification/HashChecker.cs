using Microsoft.Extensions.Logging;

namespace Belarus.Launcher.Core.FileHashVerification;

public class HashChecker(ILogger<HashChecker>? logger, IHashProvider hashProvider)
{
    private readonly ILogger<HashChecker>? _logger = logger;
    private readonly IHashProvider _hashProvider = hashProvider;

    public async Task<bool> VerifyFileHashAsync(string filePath, string expectedHash,
        CancellationToken cancellationToken = default)
    {
        await using var stream = File.OpenRead(filePath);
        var actualHash = await _hashProvider.CalculateHashAsync(stream, cancellationToken);
        _logger?.LogInformation("File {FileName} {Length} Kb ({HashBytes})", Path.GetFileName(filePath), stream.Length / 1000.0f, actualHash);

        return actualHash == expectedHash;
    }

    public bool VerifyFileHash(string filePath, string expectedHash)
    {
        using var stream = File.OpenRead(filePath);
        var actualHash = _hashProvider.CalculateHash(stream);
        _logger?.LogInformation("File {FileName} {Length} Kb ({HashBytes})", Path.GetFileName(filePath), stream.Length / 1000.0f, actualHash);

        return actualHash == expectedHash;
    }

    public async Task<bool> VerifyFileHashAsync(FileStream stream, string expectedHash, CancellationToken cancellationToken = default)
    {
        var actualHash = await _hashProvider.CalculateHashAsync(stream, cancellationToken);
        _logger?.LogInformation("File {FileName} {Length} Kb ({HashBytes})", Path.GetFileName(stream.Name), stream.Length / 1000.0f, actualHash);

        return actualHash == expectedHash;
    }

    public bool VerifyFileHash(FileStream stream, string expectedHash)
    {
        var actualHash = _hashProvider.CalculateHash(stream);
        _logger?.LogInformation("File {FileName} {Length} Kb ({HashBytes})", Path.GetFileName(stream.Name), stream.Length / 1000.0f, actualHash);

        return actualHash == expectedHash;
    }
}
