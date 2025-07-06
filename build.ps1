#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Build script for KoikatuMCP

.DESCRIPTION
    This script provides build automation for the KoikatuMCP project.
    
.PARAMETER Configuration
    The build configuration (Debug or Release). Default is Release.

.PARAMETER Clean
    Perform a clean build by removing the bin and obj directories first.

.PARAMETER Publish
    Publish the application for distribution.

.PARAMETER Runtime
    Target runtime for publish (e.g., win-x64, linux-x64, osx-x64).

.PARAMETER SelfContained
    Create a self-contained deployment.

.PARAMETER Format
    Format the code using dotnet format.

.EXAMPLE
    .\build.ps1
    Builds the project in Release configuration

.EXAMPLE
    .\build.ps1 -Configuration Debug -Clean
    Performs a clean build in Debug configuration

.EXAMPLE
    .\build.ps1 -Publish -Runtime win-x64 -SelfContained
    Publishes a self-contained Windows x64 version

.EXAMPLE
    .\build.ps1 -Format
    Formats the code without building
#>

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",
    
    [Parameter(Mandatory=$false)]
    [switch]$Clean,
    
    [Parameter(Mandatory=$false)]
    [switch]$Publish,
    
    [Parameter(Mandatory=$false)]
    [string]$Runtime,
    
    [Parameter(Mandatory=$false)]
    [switch]$SelfContained,
    
    [Parameter(Mandatory=$false)]
    [switch]$Format
)

# Set error action preference
$ErrorActionPreference = "Stop"

# Get script directory and project root
$ScriptDir = $PSScriptRoot
$ProjectRoot = $ScriptDir
$SourceDir = Join-Path $ProjectRoot "src"
$ProjectFile = Join-Path $SourceDir "KoikatuMCP.csproj"

Write-Host "=== KoikatuMCP Build Script ===" -ForegroundColor Green
Write-Host "Project Root: $ProjectRoot" -ForegroundColor Cyan
Write-Host "Configuration: $Configuration" -ForegroundColor Cyan
if ($Format) {
    Write-Host "Operation: Format" -ForegroundColor Cyan
}

# Function to find .NET SDK
function Find-DotNetSDK {
    $dotnetPaths = @(
        "${env:ProgramFiles}\dotnet\dotnet.exe",
        "${env:ProgramFiles(x86)}\dotnet\dotnet.exe",
        "${env:USERPROFILE}\.dotnet\dotnet.exe",
        "dotnet"  # Try from PATH
    )
    
    foreach ($path in $dotnetPaths) {
        try {
            if ($path -eq "dotnet") {
                $version = & dotnet --version 2>$null
                if ($LASTEXITCODE -eq 0) {
                    return "dotnet"
                }
            } elseif (Test-Path $path) {
                $version = & $path --version 2>$null
                if ($LASTEXITCODE -eq 0) {
                    return $path
                }
            }
        } catch {
            continue
        }
    }
    
    throw ".NET SDK not found. Please install .NET 8.0 SDK from https://dotnet.microsoft.com/download/dotnet/8.0"
}

# Verify .NET SDK is installed
try {
    $dotnetPath = Find-DotNetSDK
    $dotnetVersion = & $dotnetPath --version
    Write-Host "Using .NET SDK version: $dotnetVersion" -ForegroundColor Green
    Write-Host "Found at: $dotnetPath" -ForegroundColor Gray
} catch {
    Write-Error $_.Exception.Message
    exit 1
}

# Verify project file exists
if (!(Test-Path $ProjectFile)) {
    Write-Error "Project file not found at: $ProjectFile"
    exit 1
}

# Clean build if requested
if ($Clean) {
    Write-Host "Cleaning project..." -ForegroundColor Yellow
    
    $BinDir = Join-Path $SourceDir "bin"
    $ObjDir = Join-Path $SourceDir "obj"
    
    if (Test-Path $BinDir) {
        Remove-Item $BinDir -Recurse -Force
        Write-Host "Removed: $BinDir" -ForegroundColor Gray
    }
    
    if (Test-Path $ObjDir) {
        Remove-Item $ObjDir -Recurse -Force
        Write-Host "Removed: $ObjDir" -ForegroundColor Gray
    }
    
    Write-Host "Clean completed!" -ForegroundColor Green
}

# Format code if requested
if ($Format) {
    Write-Host "Formatting code..." -ForegroundColor Yellow
    
    & $dotnetPath format $ProjectFile --verbosity minimal
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Code formatting failed!"
        exit $LASTEXITCODE
    }
    
    Write-Host "Code formatting completed!" -ForegroundColor Green
    Write-Host ""
    Write-Host "=== Format Summary ===" -ForegroundColor Green
    Write-Host "Status: SUCCESS" -ForegroundColor Green
    exit 0
}

# Restore packages
Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
& $dotnetPath restore $ProjectFile
if ($LASTEXITCODE -ne 0) {
    Write-Error "Package restore failed!"
    exit $LASTEXITCODE
}

if ($Publish) {
    Write-Host "Publishing application..." -ForegroundColor Yellow
    
    $PublishArgs = @(
        "publish"
        $ProjectFile
        "--configuration", $Configuration
        "--verbosity", "minimal"
    )
    
    if ($Runtime) {
        $PublishArgs += "--runtime", $Runtime
        Write-Host "Target Runtime: $Runtime" -ForegroundColor Cyan
    }
    
    if ($SelfContained) {
        $PublishArgs += "--self-contained"
        Write-Host "Self-contained: Yes" -ForegroundColor Cyan
    }
    
    & $dotnetPath @PublishArgs
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Publish failed!"
        exit $LASTEXITCODE
    }
    
    Write-Host "Publish completed!" -ForegroundColor Green
    
    # Show output location
    $OutputPath = Join-Path $SourceDir "bin" $Configuration "net8.0"
    if ($Runtime) {
        $OutputPath = Join-Path $OutputPath $Runtime
        if ($Publish) {
            $OutputPath = Join-Path $OutputPath "publish"
        }
    }
    
    if (Test-Path $OutputPath) {
        Write-Host "Output location: $OutputPath" -ForegroundColor Cyan
        $Files = Get-ChildItem $OutputPath -File
        Write-Host "Generated files:" -ForegroundColor Cyan
        foreach ($File in $Files) {
            Write-Host "  - $($File.Name)" -ForegroundColor Gray
        }
    }
    
} else {
    Write-Host "Building application..." -ForegroundColor Yellow
    
    & $dotnetPath build $ProjectFile --configuration $Configuration --verbosity minimal
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Build failed!"
        exit $LASTEXITCODE
    }
    
    Write-Host "Build completed!" -ForegroundColor Green
}

Write-Host ""
Write-Host "=== Build Summary ===" -ForegroundColor Green
Write-Host "Configuration: $Configuration" -ForegroundColor White
Write-Host "Operation: $(if ($Publish) { 'Publish' } else { 'Build' })" -ForegroundColor White
if ($Runtime) {
    Write-Host "Runtime: $Runtime" -ForegroundColor White
}
if ($SelfContained) {
    Write-Host "Self-contained: Yes" -ForegroundColor White
}
Write-Host "Status: SUCCESS" -ForegroundColor Green