using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AbleSharp.GUI.ViewModels.Tools;
using Avalonia.Input;

namespace AbleSharp.GUI.Views.Tools;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
        DataContext = new SettingsViewModel();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == Key.Escape) Close();
    }
}