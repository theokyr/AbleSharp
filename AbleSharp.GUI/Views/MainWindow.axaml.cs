using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AbleSharp.GUI.ViewModels;
using Avalonia;

namespace AbleSharp.GUI.Views;

public partial class MainWindow : Window
{
    public MainWindow(string? loadProjectPath = null)
    {
        InitializeComponent();

        DataContext = new MainWindowViewModel(loadProjectPath);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
#if DEBUG
        this.AttachDevTools();
#endif
    }
}