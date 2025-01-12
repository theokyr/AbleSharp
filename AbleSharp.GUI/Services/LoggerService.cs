using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using Avalonia.Threading;

namespace AbleSharp.GUI.Services;

/// <summary>
/// A static service to provide a shared ILoggerFactory and log store.
/// This sets up console logging plus our custom DebugLogProvider
/// that writes logs into an in-memory list (for the DebugLog window).
/// </summary>
public static class LoggerService
{
    private static ILoggerFactory? _loggerFactory;

    /// <summary>
    /// A thread-safe collection of log entries for display in the DebugLog window.
    /// </summary>
    public static ObservableCollection<string> InMemoryLog { get; } = new();

    public static void Initialize()
    {
        if (_loggerFactory != null)
            return; // Already initialized

        // Create a logger factory that writes to console + debug provider.
        _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .SetMinimumLevel(LogLevel.Trace)
                .AddConsole() // Writes logs to console
                .AddProvider(new DebugLogProvider()); // Writes logs to InMemoryLog
        });
    }

    /// <summary>
    /// Retrieve a typed logger for use in any class.
    /// </summary>
    public static ILogger<T> GetLogger<T>()
    {
        return _loggerFactory?.CreateLogger<T>()
               ?? throw new InvalidOperationException("LoggerService not initialized.");
    }
}

/// <summary>
/// A provider that creates an ILogger which appends log messages to LoggerService.InMemoryLog.
/// </summary>
internal class DebugLogProvider : ILoggerProvider
{
    private readonly DebugLogLogger _logger = new();

    public ILogger CreateLogger(string categoryName)
    {
        return _logger;
    }

    public void Dispose()
    {
    }
}

/// <summary>
/// An ILogger implementation that appends to LoggerService.InMemoryLog.
/// </summary>
internal class DebugLogLogger : ILogger
{
    public IDisposable BeginScope<TState>(TState state)
    {
        return NullScope.Instance;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

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

        if (exception != null) final += Environment.NewLine + exception;

        // Ensure we add to the collection on the UI thread.
        Dispatcher.UIThread.Post(() => { LoggerService.InMemoryLog.Add(final); });
    }

    private class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new();

        public void Dispose()
        {
        }
    }
}