namespace Belarus.Launcher.Core.FileHashVerification;

public interface IHashProvider
{
    Task<string> CalculateHashAsync(Stream stream, CancellationToken cancellationToken = default);

    string CalculateHash(Stream stream);
}
