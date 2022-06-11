﻿using StalkerBelarus.Launcher.Helpers;
using System.Reactive;
using System.Reactive.Linq;

namespace StalkerBelarus.Launcher.ViewModels
{
    public class AuthorizationViewModel : ReactiveObject, IRoutableViewModel
    {
        public ReactiveCommand<Unit, Unit> Next { get; }

        public ReactiveCommand<Unit, Unit> Close { get; }

        public AuthorizationViewModel(IScreen screen)
        {
            HostScreen = screen;

            Next = ReactiveCommand.CreateFromTask(async () => await NextImpl());

            Close = ReactiveCommand.Create(() => ApplicationHelper.Close());
        }

        private IObservable<Unit> NextImpl()
        {
            HostScreen.Router.Navigate.Execute(new LauncherViewModel(HostScreen));

            return Observable.Return(Unit.Default);
        }

        public string? UrlPathSegment => throw new NotImplementedException();

        public IScreen HostScreen { get; protected set; }
    }
}