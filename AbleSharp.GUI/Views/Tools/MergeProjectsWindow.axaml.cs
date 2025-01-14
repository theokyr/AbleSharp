using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using AbleSharp.GUI.ViewModels.Tools;
using Microsoft.Extensions.Logging;
using AbleSharp.GUI.Services;

namespace AbleSharp.GUI.Views.Tools;

public partial class MergeProjectsWindow : Window
{
    private readonly MergeProjectsViewModel _viewModel;
    private readonly ILogger<MergeProjectsWindow> _logger;

    public MergeProjectsWindow()
    {
        InitializeComponent();
        _viewModel = DataContext as MergeProjectsViewModel ?? new MergeProjectsViewModel();
        DataContext = _viewModel;
        _logger = LoggerService.GetLogger<MergeProjectsWindow>();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    // Handle drag and drop
    private async void OnDragOver(object sender, DragEventArgs e)
    {
        if (e.Data.Contains(DataFormats.FileNames))
        {
            var isAllAls = true;
            var files = e.Data.GetFileNames();
            if (files != null)
            {
                foreach (var file in files)
                    if (!file.EndsWith(".als", StringComparison.OrdinalIgnoreCase))
                    {
                        isAllAls = false;
                        break;
                    }

                if (isAllAls)
                {
                    e.DragEffects = DragDropEffects.Copy;
                    e.Handled = true;
                    return;
                }
            }
        }

        e.DragEffects = DragDropEffects.None;
        e.Handled = true;
    }

    private async void OnDrop(object sender, DragEventArgs e)
    {
        if (e.Data.Contains(DataFormats.FileNames))
        {
            var files = e.Data.GetFileNames();
            if (files != null)
                foreach (var file in files)
                    if (file.EndsWith(".als", StringComparison.OrdinalIgnoreCase) && !_viewModel.SelectedProjects.Contains(file))
                    {
                        _viewModel.SelectedProjects.Add(file);
                        _logger.LogInformation($"Added project via drag and drop: {file}");
                    }
        }
    }
}