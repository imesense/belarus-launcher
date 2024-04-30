namespace ImeSense.Launchers.Belarus.Core.Storage;

public static class RegexPatternStorage
{
    public const string UsernamePattern = "^[a-zA-Z]+$";
    public const string IpAddressPattern = @"^(?:(?:https?|ftp):\/\/)?(?:www\.)?([a-zA-Z0-9-]+\.?)+[a-zA-Z]{2,}(?::\d+)?(?:\/[^\s]*)?$|^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(?::\d+)?$";
}
