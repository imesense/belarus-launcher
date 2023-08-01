using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace StalkerBelarus.Launcher.Avalonia.Views; 

public partial class LinkView : UserControl {
    public LinkView() {
        InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}

