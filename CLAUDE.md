
# KKStudioSocket MCP Wrapper

## Project Overview
A C# console application that wraps KKStudioSocket with Model Context Protocol (MCP) to control Koikatsu Studio via WebSocket.

## Development, Testing, and Build Workflow

### Build Commands
```powershell
# Release build
powershell.exe -File build.ps1

# Debug build  
powershell.exe -File build.ps1 -Configuration Debug
```

### Execution and Testing
```bash
# Test procedure after Claude Code restart
mcp__koikatu-mcp__ping "test message"
```

### Log Monitoring
```bash
# Check log file (today's date)
tail -n 20 ./src/bin/Release/net8.0/koikatu-mcp-$(date +%Y%m%d).log

# Check latest log file
tail -n 20 ./src/bin/Release/net8.0/koikatu-mcp-*.log | tail -n 20

# Real-time monitoring of WebSocket send/receive data
# Automatic file output via custom ILogger provider
```

## Debugging and Troubleshooting

### Common Issues and Solutions

#### 1. Build File Lock
```
# Symptom: MSB3027 file lock error
# Cause: Claude Code executing exe file
# Solution: Close Claude Code and request user to manually run build
```

### MCP Server Development Specific Notes

#### stdio Communication Considerations
- **stdout**: MCP protocol only (JSON only)
- **stderr**: Log output only (`LogToStandardErrorThreshold` configured)
- **file**: WebSocket communication data (custom ILogger provider)

#### Log Level Configuration
```csharp
// Console output to stderr, WebSocket communication logs to file
builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Information;
});
builder.Logging.AddProvider(new FileLoggerProvider());
```

## Project Structure

### Important Files
- `src/Models/KKStudioSocketModels.cs`: KKStudioSocket-compatible command classes
- `src/Services/WebSocketService.cs`: WebSocket communication and logging
- `src/Logging/FileLogger*.cs`: Custom file logger provider
- `build.ps1`: Cross-platform build script
- `.mcp.json`: MCP configuration for Claude Code

### Configuration and Deployment
```json
// .mcp.json - Production configuration
{
  "mcpServers": {
    "koikatu-mcp": {
      "command": "src/bin/Release/net8.0/KoikatuMCP.exe",
      "env": {
        "KKSTUDIOSOCKET_URL": "ws://127.0.0.1:8765/ws"
      }
    }
  }
}
```

## Documentation Maintenance

### Multi-language README Updates
When modifying README.md, always update README.ja.md with the corresponding Japanese translation to maintain consistency between English and Japanese documentation.
