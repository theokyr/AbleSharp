using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using AbleSharp.GUI.ViewModels.Tools;
using Microsoft.Extensions.Logging;
using AbleSharp.GUI.Services;
using Avalonia.Threading;

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

        // Set up drag-drop handlers
        AddHandler(DragDrop.DragOverEvent, DragOver, handledEventsToo: true);
        AddHandler(DragDrop.DropEvent, Drop, handledEventsToo: true);
        AddHandler(DragDrop.DragLeaveEvent, DragLeave, handledEventsToo: true);

#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void DragOver(object? sender, DragEventArgs e)
    {
        if (e.Data.Contains(DataFormats.FileNames))
        {
            var files = e.Data.GetFileNames();
            if (files != null && files.All(file => file.EndsWith(".als", StringComparison.OrdinalIgnoreCase)))
            {
                e.DragEffects = DragDropEffects.Copy;
                _viewModel.IsDragOver = true;
                e.Handled = true;
                return;
            }
        }

        e.DragEffects = DragDropEffects.None;
        _viewModel.IsDragOver = false;
        e.Handled = true;
    }

    private async void Drop(object? sender, DragEventArgs e)
    {
        _viewModel.IsDragOver = false; // Reset drag state on drop

        if (e.Data.Contains(DataFormats.FileNames))
        {
            var files = e.Data.GetFileNames();
            if (files != null)
            {
                var alsFiles = files.Where(file => file.EndsWith(".als", StringComparison.OrdinalIgnoreCase)).ToList();

                if (alsFiles.Any())
                    foreach (var file in alsFiles)
                        // Ensure AddProject is executed on the UI thread
                        await Dispatcher.UIThread.InvokeAsync(() => _viewModel.AddProject(file));
                else
                    // Notify the user about invalid files
                    await ShowInvalidFileTypesMessage();
            }
        }
    }

    private void DragLeave(object? sender, DragEventArgs e)
    {
        _viewModel.IsDragOver = false;
    }

    private async Task ShowInvalidFileTypesMessage()
    {
        var dialog = new Window
        {
            Title = "Invalid Files",
            Width = 400,
            Height = 200,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new TextBlock
            {
                Text = "Only .als files are supported.",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap
            }
        };

        await dialog.ShowDialog(this);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == Key.Escape) Close();
    }
}