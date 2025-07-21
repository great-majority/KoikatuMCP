# Contributing to KoikatuMCP

## Development Setup

### Prerequisites
- **Windows** (primary development platform)
- **.NET 8.0 SDK** or later
- **PowerShell** (for build scripts)
- **Node.js and npm** (for DXT packaging with `@anthropic-ai/dxt`)
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

# Debug build
.\build.ps1 build Debug

# Clean all outputs
.\build.ps1 clean

# Rebuild (clean + build)
.\build.ps1 rebuild

# Format code
.\build.ps1 format

# Publish application
.\build.ps1 publish
```

### DXT Packaging (Claude Desktop Extension)
```powershell
# Install DXT CLI tool
npm install -g @anthropic-ai/dxt

# Prepare DXT package directory
.\build.ps1 build-dxt

# Create .dxt package file
.\build.ps1 create-dxt

# Full DXT build workflow
.\build.ps1 dxt
```

### Build Script Targets
- `clean` - Remove all build outputs
- `restore` - Restore NuGet packages
- `build` - Build the solution (default)
- `rebuild` - Clean + restore + build
- `publish` - Publish the application
- `format` - Format code using dotnet format
- `build-dxt` - Prepare DXT package directory
- `create-dxt` - Create .dxt package file
- `dxt` - Complete DXT build and packaging workflow
