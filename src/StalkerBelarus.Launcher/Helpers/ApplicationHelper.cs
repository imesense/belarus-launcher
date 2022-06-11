namespace StalkerBelarus.Launcher.Helpers;

public static class ApplicationHelper
{
    public static void Close()
    {
        Application.Current.Shutdown();
    }
}