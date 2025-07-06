# Contributing to KoikatuMCP

## Development Setup

### Prerequisites
- **Windows** (primary development platform)
- **.NET 8.0 SDK** or later
- **PowerShell** (for build scripts)
- **Git**
- **Koikatsu/Koikatsu Sunshine** with KKStudioSocket plugin (for testing)

### Getting Started
```bash
# Clone the repository
git clone https://github.com/great-majority/KoikatuMCP.git
cd KoikatuMCP

# Build the project
.\build.ps1

# Run in development mode
.\scripts\run.ps1 -Configuration Debug -Build
```

## Project Structure

```
KoikatuMCP/
├── src/
│   ├── KoikatuMCP.csproj          # Main project file
│   ├── Program.cs                  # Application entry point
│   ├── Models/
│   │   └── KKStudioSocketModels.cs # Command models for WebSocket
│   ├── Services/
│   │   └── WebSocketService.cs     # WebSocket communication service
│   ├── Tools/
│   │   ├── KoikatuStudioTools.cs   # Core studio manipulation tools
│   │   ├── KoikatuCameraTools.cs   # Camera control tools
│   │   ├── KoikatuItemCatalogTools.cs # Item catalog browsing tools
│   │   ├── KoikatuScreenshotTools.cs  # Screenshot capture tools
│   │   └── Models/
│   │       └── WebSocketModels.cs   # Response models for WebSocket
│   └── Logging/
│       ├── FileLogger.cs           # Custom file logger
│       └── FileLoggerProvider.cs   # Logger provider
├── .github/
│   └── workflows/
│       └── ci.yml                  # CI/CD pipeline
├── CLAUDE.md                       # Development documentation for Claude
├── build.ps1
└── README.md                       # User documentation
```

## Development Workflow

### Building
```powershell
# Standard build
.\build.ps1

# Clean build
.\build.ps1 -Clean

# Debug build
.\build.ps1 -Configuration Debug

# Format code
.\build.ps1 -Format

# Publish single-file executable
.\build.ps1 -Publish -Runtime win-x64 -SelfContained
```
