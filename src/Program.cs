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
    options.LogToStandardErrorThreshold = LogLevel.Information;
});

// Add custom file logging
builder.Logging.AddProvider(new FileLoggerProvider());

var host = builder.Build();

await host.RunAsync();