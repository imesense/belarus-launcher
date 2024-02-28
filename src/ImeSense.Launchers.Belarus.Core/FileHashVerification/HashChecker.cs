using Microsoft.Extensions.Logging;

namespace ImeSense.Launchers.Belarus.Core.FileHashVerification;

public class HashChecker {
    private readonly ILogger<HashChecker>? _logger;
    private readonly IHashProvider _hashProvider;

    public HashChecker(ILogger<HashChecker>? logger, IHashProvider hashProvider) {
        _logger = logger;
        _hashProvider = hashProvider;
    }

    public async Task<bool> VerifyFileHashAsync(string filePath, string expectedHash, 
        CancellationToken cancellationToken = default) {
        await using var stream = File.OpenRead(filePath);
        var actualHash = await _hashProvider.CalculateHashAsync(stream, cancellationToken);
        _logger?.LogInformation("File {FileName} {Length} Kb ({HashBytes})", Path.GetFileName(filePath), stream.Length / 1000.0f, actualHash);
        return actualHash == expectedHash;
    }
}
