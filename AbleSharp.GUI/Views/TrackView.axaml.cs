using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AbleSharp.GUI.Views;

public partial class TrackView : UserControl
{
    public TrackView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}