namespace StalkerBelarus.Launcher.Core.FileHashVerification;

public class HashChecker {
    private readonly IHashProvider _hashProvider;

    public HashChecker(IHashProvider hashProvider) {
        _hashProvider = hashProvider;
    }

    public async Task<bool> VerifyFileHashAsync(string filePath, string expectedHash) {
        var actualHash = await _hashProvider.CalculateHashAsync(filePath);
        return actualHash == expectedHash;
    }
}
