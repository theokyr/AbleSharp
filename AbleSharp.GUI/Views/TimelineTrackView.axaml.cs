using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AbleSharp.GUI.Views;

public partial class TimelineTrackView : UserControl
{
    public TimelineTrackView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}