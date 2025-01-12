using System.Collections.ObjectModel;
using AbleSharp.GUI.Services;

namespace AbleSharp.GUI.ViewModels
{
    /// <summary>
    /// Simple ViewModel for the DebugLogWindow.
    /// Binds to LoggerService.InMemoryLog to show the logs in real-time.
    /// </summary>
    public class DebugLogWindowViewModel
    {
        public ObservableCollection<string> LogEntries => LoggerService.InMemoryLog;
    }
}