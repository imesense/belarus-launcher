using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace StalkerBelarus.Launcher.Avalonia.Views; 

public partial class LauncherView : UserControl {
    public LauncherView() {
        InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}

