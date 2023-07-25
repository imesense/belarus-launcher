using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace StalkerBelarus.Launcher.Avalonia.Views; 

public partial class NewsView : UserControl {
    public NewsView() {
        InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}

