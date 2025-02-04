using System.Globalization;
using System.Text;

namespace ImeSense.Launchers.Belarus.Core.Logger;

public static class InformationPrinter
{
    public static string GetStartupInfo(string nameApp)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"Start {nameApp}")
            .AppendLine();
        builder.AppendLine(GetOsInfo());
        builder.AppendLine(GetApplicationInfo());

        return builder.ToString();
    }

    public static string GetOsInfo()
    {
        var builder = new StringBuilder();
        builder.AppendLine("Basic information about operating system")
             .AppendLine();
        builder.AppendLine($"OS version: {Environment.OSVersion}");
        builder.AppendLine($"Processor architecture: {Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE")}");
        builder.AppendLine($"Processor count: {Environment.ProcessorCount}");
        builder.AppendLine($"User / PC: {Environment.UserName} / {Environment.MachineName}");
        builder.AppendLine($"System directory: {Environment.SystemDirectory}");
        builder.AppendLine($"Windows locale: {CultureInfo.CurrentCulture.ThreeLetterWindowsLanguageName}");

        return builder.ToString();
    }

    public static string GetApplicationInfo()
    {
        var builder = new StringBuilder();
        builder.AppendLine("Basic application information")
            .AppendLine();
        builder.AppendLine($".NET: {Environment.Version}");
        builder.AppendLine($"ProcessId: {Environment.ProcessId}");
        builder.AppendLine($"Process path: {Environment.ProcessPath}");
        builder.AppendLine($"Working directory: {Environment.CurrentDirectory}");
        builder.Append($"Application directory: {AppDomain.CurrentDomain.BaseDirectory}");

        var commandLineArgs = Environment.GetCommandLineArgs();
        if (commandLineArgs.Length > 1)
        {
            builder.AppendLine("Command line arguments:");
            foreach (var arg in commandLineArgs)
            {
                builder.AppendLine(arg);
            }
        }

        return builder.ToString();
    }
}
