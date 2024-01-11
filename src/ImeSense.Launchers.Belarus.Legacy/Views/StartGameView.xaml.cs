namespace ImeSense.Launchers.Belarus.Views;

/// <summary>
/// Interaction logic for StartGameView.xaml
/// </summary>
public partial class StartGameView : IViewFor<StartGameViewModel> {
    public StartGameView() {
        InitializeComponent();

        this.WhenActivated(d => {
            d(this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext));
            d(this.BindCommand(ViewModel, vm => vm.StartGame, view => view.BtnStartGame));
            d(this.BindCommand(ViewModel, vm => vm.Back, view => view.BtnBack));
            d(this.Bind(ViewModel, vm => vm.IpAddress, view => view.IpAddress.Text));
        });
    }

    public StartGameViewModel? ViewModel {
        get => (StartGameViewModel) GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register("ViewModel", typeof(StartGameViewModel), typeof(StartGameView), new PropertyMetadata(null));

    object? IViewFor.ViewModel {
        get { return ViewModel; }
        set { ViewModel = (StartGameViewModel?) value; }
    }
}
