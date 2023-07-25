using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace StalkerBelarus.Launcher.Avalonia.Views; 

public partial class StartGameView : UserControl {
    public StartGameView() {
        InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}

