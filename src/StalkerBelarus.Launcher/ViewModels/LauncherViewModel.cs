﻿namespace StalkerBelarus.Launcher.ViewModels;

public class LauncherViewModel : ReactiveObject, IRoutableViewModel
{
    public string? UrlPathSegment => throw new NotImplementedException();

    public IScreen HostScreen { get; set; } = null!;
}
