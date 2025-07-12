using KoikatuMCP.Logging;
using KoikatuMCP.Services;
using KoikatuMCP.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

builder.Services.AddSingleton<WebSocketService>();

builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Debug;
});

// Add custom file logging with Debug level
builder.Logging.AddProvider(new FileLoggerProvider(LogLevel.Information));
builder.Logging.SetMinimumLevel(LogLevel.Information);

var host = builder.Build();

await host.RunAsync();