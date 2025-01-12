using Avalonia.Platform.Storage;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace AbleSharp.GUI.Services;

public static class FileDialogService
{
    public static async Task<string?> ShowOpenFileDialogAsync()
    {
        var topLevelWindow = Application.Current?.ApplicationLifetime switch
        {
            IClassicDesktopStyleApplicationLifetime desktop => desktop.MainWindow,
            _ => null
        };

        if (topLevelWindow?.StorageProvider is null)
            return null;

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

        var storageResults = await topLevelWindow.StorageProvider.OpenFilePickerAsync(options);

        if (storageResults?.Count > 0)
        {
            var localPath = storageResults[0].TryGetLocalPath();
            return localPath;
        }

        return null;
    }
}