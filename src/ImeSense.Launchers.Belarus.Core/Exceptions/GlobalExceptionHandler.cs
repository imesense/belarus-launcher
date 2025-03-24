using Microsoft.Extensions.Logging;

using ReactiveUI;
using System.Reactive;
using System.Text;

namespace ImeSense.Launchers.Belarus.Core.Exceptions;

public static class GlobalExceptionHandler
{
    private static ILogger? _logger;

    public static void Initialize(ILogger? logger)
    {
        _logger = logger;

        // ReactiveUI: обработка исключений
        RxApp.DefaultExceptionHandler = Observer.Create<Exception>(LogException);

        // TaskScheduler: необработанные исключения из задач
        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            LogException(e.Exception);
            e.SetObserved(); // Указываем, что исключение обработано
        };

        // AppDomain: необработанные исключения из основного потока
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            if (e.ExceptionObject is Exception ex)
            {
                LogException(ex);
                Environment.Exit(1);
            }
        };
    }

    private static void LogException(Exception ex)
    {
        ArgumentNullException.ThrowIfNull(ex);

        var builder = new StringBuilder();
        builder.AppendLine($"Unhandled exception occurred: {ex.GetType().FullName} - {ex.Message}");
        builder.AppendLine($"Stack Trace: {ex.StackTrace ?? "No stack trace available"}");

        if (ex.InnerException != null)
        {
            builder.AppendLine($"Inner Exception: {ex.InnerException.GetType().FullName} - {ex.InnerException.Message}");
            builder.AppendLine($"Inner Stack Trace: {ex.InnerException.StackTrace ?? "No inner stack trace available"}");
        }

        _logger?.LogError("Exception details: {Exception}", builder.ToString());
    }
}
