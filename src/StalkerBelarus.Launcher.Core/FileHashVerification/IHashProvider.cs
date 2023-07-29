namespace StalkerBelarus.Launcher.Core.FileHashVerification; 

public interface IHashProvider {
    Task<string> CalculateHashAsync(string filePath);
}
