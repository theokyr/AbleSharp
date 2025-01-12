using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AbleSharp.GUI.ViewModels;

namespace AbleSharp.GUI.Views;

public partial class DebugLogWindow : Window
{
    public DebugLogWindow()
    {
        InitializeComponent();
        DataContext = new DebugLogWindowViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}