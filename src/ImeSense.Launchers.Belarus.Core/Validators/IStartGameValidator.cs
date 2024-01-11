namespace ImeSense.Launchers.Belarus.Core.Validators;

public interface IStartGameValidator {
    bool IsIpAddressNotEmpty(string serverAddress);
    bool IsValidIpAddressOrUrl(string serverAddress);
}
