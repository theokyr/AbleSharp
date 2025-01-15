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

    public MergeProjectsWindow(MergeProjectsViewModel? viewModel = null)
    {
        _viewModel = viewModel ?? new MergeProjectsViewModel();
        DataContext = _viewModel;
        _logger = LoggerService.GetLogger<MergeProjectsWindow>();
        InitializeComponent();

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
            _logger.LogDebug($"[MergeProjectsWindow] Setting up drag-drop handlers for FileDropZone");

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
        _logger.LogDebug($"[MergeProjectsWindow] DragEnter event received");
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
        _logger.LogDebug($"[MergeProjectsWindow] DragLeave event received");
        _viewModel.IsDragOver = false;
        e.Handled = true;
    }

    private (bool IsValid, IEnumerable<string> ValidFiles, IEnumerable<string> InvalidFiles) ValidateAndCategorizeFiles(DragEventArgs e)
    {
        _logger.LogDebug($"[MergeProjectsWindow] Validating drag data: {e.Data.GetType()}");

        if (!e.Data.Contains(DataFormats.Files))
        {
            _logger.LogDebug($"[MergeProjectsWindow] Drag data does not contain file names");
            return (false, Enumerable.Empty<string>(), Enumerable.Empty<string>());
        }

        var files = e.Data.GetFiles();
        if (files == null || !files.Any())
        {
            _logger.LogDebug($"[MergeProjectsWindow] No files found in drag data");
            return (false, Enumerable.Empty<string>(), Enumerable.Empty<string>());
        }

        var validFiles = files.Select(file => file.Path.AbsolutePath.ToString())
            .Where(file => file.EndsWith(".als", StringComparison.OrdinalIgnoreCase));
        var invalidFiles = files.Select(file => file.Path.AbsolutePath.ToString())
            .Where(file => !file.EndsWith(".als", StringComparison.OrdinalIgnoreCase));

        _logger.LogDebug($"[MergeProjectsWindow] Valid files: {string.Join(", ", validFiles)}");
        _logger.LogDebug($"[MergeProjectsWindow] Invalid files: {string.Join(", ", invalidFiles)}");

        return (validFiles.Any(), validFiles, invalidFiles);
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        var (isValid, _, _) = ValidateAndCategorizeFiles(e);

        if (isValid)
        {
            if (!_viewModel.IsDragOver) _logger.LogDebug($"[MergeProjectsWindow] Valid ALS files detected during DragOver");

            _viewModel.IsDragOver = true;
            e.DragEffects = DragDropEffects.Copy;
        }
        else
        {
            if (_viewModel.IsDragOver) _logger.LogDebug($"[MergeProjectsWindow] Invalid files detected during DragOver");

            _viewModel.IsDragOver = false;
            e.DragEffects = DragDropEffects.None;
        }

        e.Handled = true;
    }

    private async void OnDrop(object? sender, DragEventArgs e)
    {
        try
        {
            _logger.LogDebug($"[MergeProjectsWindow] Drop event received");
            _viewModel.IsDragOver = false;

            var (isValid, validFiles, invalidFiles) = ValidateAndCategorizeFiles(e);

            if (!isValid)
            {
                _logger.LogWarning("[MergeProjectsWindow] Drop data contains no valid files");
                return;
            }

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var projectPaths = validFiles.ToList();
                foreach (var file in projectPaths)
                {
                    _logger.LogInformation($"[MergeProjectsWindow] Adding dropped file: {file}");
                }

                _logger.LogDebug($"[MergeProjectsWindow] DROP ViewModel instance: {_viewModel.GetHashCode()}");
                _viewModel.AddProjects(projectPaths);
            });

            if (!invalidFiles.Any())
            {
                return;
            }

            _logger.LogWarning($"[MergeProjectsWindow] Skipped {invalidFiles.Count()} non-ALS files");
            await ShowInvalidFileTypesMessage(invalidFiles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[MergeProjectsWindow] Error handling file drop");
        }
        finally
        {
            e.Handled = true;
        }
    }

    private bool ValidateDragData(DragEventArgs e)
    {
        var (isValid, _, _) = ValidateAndCategorizeFiles(e);
        return isValid;
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