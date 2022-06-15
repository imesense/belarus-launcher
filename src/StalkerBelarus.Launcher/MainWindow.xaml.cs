using Splat;

namespace StalkerBelarus.Launcher;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = Locator.Current.GetService<IScreen>();
    }
}
