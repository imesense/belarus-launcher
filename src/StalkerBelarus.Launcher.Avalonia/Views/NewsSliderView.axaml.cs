using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace StalkerBelarus.Launcher.Avalonia.Views; 

public partial class NewsSliderView : UserControl {
    public NewsSliderView() {
        InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}

