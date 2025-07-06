using Microsoft.Extensions.Logging;

namespace KoikatuMCP.Logging;

public class FileLoggerProvider : ILoggerProvider
{
    private readonly string _logFilePath;

    public FileLoggerProvider()
    {
        var exeDir = Path.GetDirectoryName(Environment.ProcessPath) ?? Directory.GetCurrentDirectory();
        _logFilePath = Path.Combine(exeDir, $"koikatu-mcp-{DateTime.Now:yyyyMMdd}.log");
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(categoryName, _logFilePath);
    }

    public void Dispose()
    {
        // Nothing to dispose
    }
}