using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using Avalonia.Threading;

namespace AbleSharp.GUI.Services;

public static class LoggerService
{
    private static ILoggerFactory? _loggerFactory;
    public static ObservableCollection<string> InMemoryLog { get; } = new();

    public static void Initialize()
    {
        if (_loggerFactory != null)
            return;

        var logLevel = 
#if DEBUG
            LogLevel.Debug;
#else
            LogLevel.Warning; // Only warnings and errors in production
#endif

        _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .SetMinimumLevel(logLevel)
                .AddFilter("AbleSharp.GUI.Views.TimelineView", LogLevel.Warning)
                .AddFilter("AbleSharp.GUI.Views.TimeRulerView", LogLevel.Warning)
                .AddFilter("AbleSharp.GUI.Views.TimelineTrackView", LogLevel.Warning)
                .AddFilter("AbleSharp.GUI.ViewModels.TimelineClipViewModel", LogLevel.Warning)
                .AddFilter("AbleSharp.GUI.ViewModels.TimelineTrackViewModel", LogLevel.Warning)
                .AddFilter("AbleSharp.GUI.ViewModels.TimelineViewModel", LogLevel.Warning)
                .AddConsole();

#if DEBUG
            builder.AddProvider(new DebugLogProvider());
#endif
        });
    }

    public static ILogger<T> GetLogger<T>()
    {
        if (_loggerFactory == null)
            Initialize();

        return _loggerFactory!.CreateLogger<T>();
    }
}

internal class DebugLogProvider : ILoggerProvider
{
    private readonly DebugLogLogger _logger = new();

    public ILogger CreateLogger(string categoryName) => _logger;

    public void Dispose() { }
}

internal class DebugLogLogger : ILogger
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        var message = formatter(state, exception);
        var final = $"{DateTime.Now:HH:mm:ss} [{logLevel}] {message}";

        if (exception != null)
            final += Environment.NewLine + exception;

        Dispatcher.UIThread.Post(() => LoggerService.InMemoryLog.Add(final));
    }
}