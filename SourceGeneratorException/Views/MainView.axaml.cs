using Avalonia.Controls;

namespace SourceGeneratorException.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        ComboBox1.IsEnabled = false;
    }
}
