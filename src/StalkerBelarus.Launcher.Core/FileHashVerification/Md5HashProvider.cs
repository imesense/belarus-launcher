using System.Security.Cryptography;

using Microsoft.Extensions.Logging;

namespace StalkerBelarus.Launcher.Core.FileHashVerification;

public class Md5HashProvider : IHashProvider {
    private readonly ILogger<Md5HashProvider> _logger;

    public Md5HashProvider(ILogger<Md5HashProvider> logger) {
        _logger = logger;
    }

    public async Task<string> CalculateHashAsync(string filePath) {
        try {
            using var md5 = MD5.Create();
            await using var stream = File.OpenRead(filePath);
            var hashBytes = await md5.ComputeHashAsync(stream);
            _logger.LogInformation("Hash {FileName} - {HashBytes}", Path.GetFileName(filePath), hashBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "");
        } catch (Exception exception) {
            _logger.LogError("{Message}", exception.Message);

            return string.Empty;
        }
    }
}
