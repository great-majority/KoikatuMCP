using Microsoft.Extensions.Logging;

namespace KoikatuMCP.Logging;

public class FileLogger : ILogger
{
    private readonly string _categoryName;
    private readonly string _logFilePath;
    private readonly LogLevel _minimumLevel;
    private static readonly object _lock = new();

    public FileLogger(string categoryName, string logFilePath, LogLevel minimumLevel)
    {
        _categoryName = categoryName;
        _logFilePath = logFilePath;
        _minimumLevel = minimumLevel;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= _minimumLevel;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        lock (_lock)
        {
            try
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                var message = formatter(state, exception);
                var logEntry = $"[{timestamp}] [{logLevel}] [{_categoryName}] {message}\n";

                File.AppendAllText(_logFilePath, logEntry);
            }
            catch
            {
                // Ignore logging errors to prevent infinite loops
            }
        }
    }
}