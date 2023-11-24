namespace StalkerBelarus.Launcher.Core.Validators;

public interface IStartGameValidator {
    bool IsIpAddressNotEmpty(string serverAddress);
    bool IsValidIpAddressOrUrl(string serverAddress);
}
