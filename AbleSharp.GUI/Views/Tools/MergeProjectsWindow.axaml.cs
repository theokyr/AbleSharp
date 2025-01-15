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
    private Border? _fileDropZone;

    public MergeProjectsWindow()
    {
        InitializeComponent();
        _viewModel = DataContext as MergeProjectsViewModel ?? new MergeProjectsViewModel();
        DataContext = _viewModel;
        _logger = LoggerService.GetLogger<MergeProjectsWindow>();

#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        // Get reference to the drop zone
        _fileDropZone = this.FindControl<Border>("FileDropZone");

        if (_fileDropZone != null)
        {
            // Set up drag-drop handlers
            _fileDropZone.AddHandler(DragDrop.DragEnterEvent, OnDragEnter);
            _fileDropZone.AddHandler(DragDrop.DragLeaveEvent, OnDragLeave);
            _fileDropZone.AddHandler(DragDrop.DragOverEvent, OnDragOver);
            _fileDropZone.AddHandler(DragDrop.DropEvent, OnDrop);
        }
        else
        {
            _logger.LogError("Failed to find FileDropZone control");
        }
    }

    private void OnDragEnter(object? sender, DragEventArgs e)
    {
        if (ValidateDragData(e))
        {
            _viewModel.IsDragOver = true;
            e.DragEffects = DragDropEffects.Copy;
        }
        else
        {
            _viewModel.IsDragOver = false;
            e.DragEffects = DragDropEffects.None;
        }

        e.Handled = true;
    }

    private void OnDragLeave(object? sender, DragEventArgs e)
    {
        _viewModel.IsDragOver = false;
        e.Handled = true;
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        if (ValidateDragData(e))
        {
            _viewModel.IsDragOver = true;
            e.DragEffects = DragDropEffects.Copy;
        }
        else
        {
            _viewModel.IsDragOver = false;
            e.DragEffects = DragDropEffects.None;
        }

        e.Handled = true;
    }

    private async void OnDrop(object? sender, DragEventArgs e)
    {
        try
        {
            _viewModel.IsDragOver = false;

            if (!e.Data.Contains(DataFormats.FileNames))
            {
                _logger.LogWarning("Drop data does not contain file names");
                return;
            }

            var files = e.Data.GetFileNames();
            if (files == null)
            {
                _logger.LogWarning("No files in drop data");
                return;
            }

            var validFiles = files.Where(f => f.EndsWith(".als", StringComparison.OrdinalIgnoreCase));
            var invalidFiles = files.Where(f => !f.EndsWith(".als", StringComparison.OrdinalIgnoreCase));

            foreach (var file in validFiles)
            {
                await Dispatcher.UIThread.InvokeAsync(() => _viewModel.AddProject(file));
                _logger.LogInformation($"Added dropped file: {file}");
            }

            if (invalidFiles.Any())
            {
                _logger.LogWarning($"Skipped {invalidFiles.Count()} non-ALS files");
                await ShowInvalidFileTypesMessage(invalidFiles);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling file drop");
        }
        finally
        {
            e.Handled = true;
        }
    }

    private bool ValidateDragData(DragEventArgs e)
    {
        if (!e.Data.Contains(DataFormats.FileNames))
            return false;

        var files = e.Data.GetFileNames();
        return files != null && files.Any(file => file.EndsWith(".als", StringComparison.OrdinalIgnoreCase));
    }

    private async Task ShowInvalidFileTypesMessage(IEnumerable<string> invalidFiles)
    {
        var fileList = string.Join("\n", invalidFiles.Select(Path.GetFileName));
        var dialog = new Window
        {
            Title = "Invalid Files",
            Width = 400,
            Height = 200,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new TextBlock
            {
                Text = $"The following files were skipped because they are not Ableton Live Sets (.als):\n\n{fileList}",
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                Margin = new Thickness(20)
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