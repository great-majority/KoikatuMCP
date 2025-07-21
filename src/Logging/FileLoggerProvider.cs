using Microsoft.Extensions.Logging;

namespace KoikatuMCP.Logging;

public class FileLoggerProvider : ILoggerProvider
{
    private readonly string _logFilePath;
    private readonly LogLevel _minimumLevel;

    public FileLoggerProvider(LogLevel minimumLevel)
    {
        var exeDir = Path.GetDirectoryName(Environment.ProcessPath) ?? Directory.GetCurrentDirectory();
        _logFilePath = Path.Combine(exeDir, $"koikatu-mcp-{DateTime.Now:yyyyMMdd}.log");
        _minimumLevel = minimumLevel;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(categoryName, _logFilePath, _minimumLevel);
    }

    public void Dispose()
    {
        // Nothing to dispose
    }
}