namespace ImeSense.Launchers.Belarus.Views;

/// <summary>
/// Interaction logic for AuthorizationView.xaml
/// </summary>
public partial class AuthorizationView : IViewFor<AuthorizationViewModel> {
    public AuthorizationView() {
        InitializeComponent();

        this.WhenActivated(d => {
            d(this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext));
            d(this.BindCommand(ViewModel, vm => vm.Next, view => view.BtnNext));
            d(this.BindCommand(ViewModel, vm => vm.Close, view => view.BtnClose));
            d(this.Bind(ViewModel, vm => vm.UserName, view => view.Username.Text));
        });
    }

    public AuthorizationViewModel? ViewModel {
        get => (AuthorizationViewModel) GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register("ViewModel", typeof(AuthorizationViewModel), typeof(AuthorizationView), new PropertyMetadata(null));

    object? IViewFor.ViewModel {
        get { return ViewModel; }
        set { ViewModel = (AuthorizationViewModel?) value; }
    }
}
