using System.Collections.Concurrent;
using ArkServerManager.Worker.Configuration;
using Microsoft.Extensions.Options;

namespace ArkServerManager.Worker.Infrastructure;

public sealed class FileLoggerProvider(
    IOptions<ManagerPathsOptions> options) : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, FileLogger> _loggers = new(StringComparer.OrdinalIgnoreCase);

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, _ => new FileLogger(categoryName, options.Value.ManagerDataRoot));
    }

    public void Dispose()
    {
        _loggers.Clear();
    }
}

internal sealed class FileLogger(string categoryName, string managerDataRoot) : ILogger
{
    private static readonly object Sync = new();

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var logsDir = Path.Combine(managerDataRoot, "logs");
        Directory.CreateDirectory(logsDir);
        var filePath = Path.Combine(logsDir, "manager.log");
        var line = $"{DateTime.UtcNow:O} [{logLevel}] {categoryName}: {formatter(state, exception)}";
        if (exception is not null)
        {
            line += $" Exception={exception.GetType().Name}:{exception.Message}";
        }

        lock (Sync)
        {
            File.AppendAllLines(filePath, [line]);
        }
    }
}
