namespace StalkerBelarus.Launcher.Core.FileHashVerification; 

public interface IHashProvider {
    Task<string> CalculateHashAsync(Stream stream, CancellationToken cancellationToken = default);
}
