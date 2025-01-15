using Avalonia.Platform.Storage;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace AbleSharp.GUI.Services;

public static class FileDialogService
{
    public static async Task<IEnumerable<string>?> ShowOpenFilesDialogAsync(FilePickerOpenOptions options)
    {
        var topLevelWindow = Application.Current?.ApplicationLifetime switch
        {
            IClassicDesktopStyleApplicationLifetime desktop => desktop.MainWindow,
            _ => null
        };

        if (topLevelWindow?.StorageProvider is null)
            return null;

        var storageResults = await topLevelWindow.StorageProvider.OpenFilePickerAsync(options);

        if (storageResults?.Count > 0)
        {
            var localPaths = storageResults.Select(sr => sr.TryGetLocalPath()).Where(p => p != null).ToList();
            return localPaths;
        }

        return null;
    }

    public static async Task<string?> ShowSaveFileDialogAsync(string defaultFileName = "project.als")
    {
        var topLevelWindow = Application.Current?.ApplicationLifetime switch
        {
            IClassicDesktopStyleApplicationLifetime desktop => desktop.MainWindow,
            _ => null
        };

        if (topLevelWindow?.StorageProvider is null)
            return null;

        var options = new FilePickerSaveOptions
        {
            Title = "Save Merged Project",
            DefaultExtension = "als",
            SuggestedFileName = defaultFileName,
            FileTypeChoices = new List<FilePickerFileType>
            {
                new("Ableton Live Set") { Patterns = new[] { "*.als" } },
                new("All Files") { Patterns = new[] { "*.*" } }
            }
        };

        var storageResult = await topLevelWindow.StorageProvider.SaveFilePickerAsync(options);

        var localPath = storageResult?.TryGetLocalPath();
        return localPath;
    }

    public static async Task<string?> ShowOpenFileDialogAsync()
    {
        var options = new FilePickerOpenOptions
        {
            Title = "Open .als File",
            AllowMultiple = false,
            FileTypeFilter = new List<FilePickerFileType>
            {
                new("Ableton Live Set") { Patterns = new[] { "*.als" } },
                new("All Files") { Patterns = new[] { "*.*" } }
            }
        };
        var filePaths = await ShowOpenFilesDialogAsync(options);
        return filePaths?.FirstOrDefault();
    }

    public static async Task<IEnumerable<string>?> ShowOpenFilesDialogAsync()
    {
        var options = new FilePickerOpenOptions
        {
            Title = "Select Project Files to Merge",
            AllowMultiple = true,
            FileTypeFilter = new List<FilePickerFileType>
            {
                new("Ableton Live Set") { Patterns = new[] { "*.als" } },
                new("All Files") { Patterns = new[] { "*.*" } }
            }
        };
        return await ShowOpenFilesDialogAsync(options);
    }
}