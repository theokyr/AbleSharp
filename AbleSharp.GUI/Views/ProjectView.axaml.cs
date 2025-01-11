using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AbleSharp.GUI.Views
{
    public partial class ProjectView : UserControl
    {
        public ProjectView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}