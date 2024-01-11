using System.Security.Cryptography;

using Microsoft.Extensions.Logging;

namespace ImeSense.Launchers.Belarus.Core.FileHashVerification;

public class Md5HashProvider : IHashProvider {
    private readonly ILogger<Md5HashProvider>? _logger;

    public Md5HashProvider(ILogger<Md5HashProvider>? logger) {
        _logger = logger;
    }

    public async Task<string> CalculateHashAsync(Stream stream, CancellationToken cancellationToken = default) {
        try {
            using var md5 = MD5.Create();
            var hashBytes = await md5.ComputeHashAsync(stream, cancellationToken);

            return BitConverter.ToString(hashBytes).Replace("-", "");
        } catch (Exception exception) {
            _logger?.LogError("{Message}", exception.Message);

            return string.Empty;
        }
    }
}
