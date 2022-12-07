namespace StalkerBelarus.Launcher.Views;

/// <summary>
/// Interaction logic for LauncherView.xaml
/// </summary>
public partial class LauncherView : IViewFor<LauncherViewModel> {
    public LauncherView() {
        InitializeComponent();

        this.WhenActivated(d => {
            d(this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext));
        });
    }

    public LauncherViewModel? ViewModel {
        get => (LauncherViewModel) GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register("ViewModel", typeof(LauncherViewModel), typeof(LauncherView), new PropertyMetadata(null));

    object? IViewFor.ViewModel {
        get => ViewModel;
        set => ViewModel = (LauncherViewModel?) value;
    }
}
