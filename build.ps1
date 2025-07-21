#!/usr/bin/env pwsh

param(
    [string]$Target = "build",
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

function Write-Banner {
    param([string]$Message)
    Write-Host "`n===============================================" -ForegroundColor Cyan
    Write-Host " $Message" -ForegroundColor Cyan
    Write-Host "===============================================`n" -ForegroundColor Cyan
}

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

function Invoke-Clean {
    Write-Banner "Cleaning build outputs"
    
    $ScriptDir = $PSScriptRoot
    $SourceDir = Join-Path $ScriptDir "src"
    $BuildDir = Join-Path $ScriptDir "build"
    
    $pathsToClean = @(
        (Join-Path $SourceDir "bin"),
        (Join-Path $SourceDir "obj")
    )
    
    foreach ($path in $pathsToClean) {
        if (Test-Path $path) {
            Remove-Item -Recurse -Force $path
            Write-Host "Deleted: $path" -ForegroundColor Yellow
        }
    }
    
    # Clean DXT build artifacts
    if (Test-Path $BuildDir) {
        Get-ChildItem $BuildDir -Filter "*.exe" -ErrorAction SilentlyContinue | Remove-Item -Force
        Get-ChildItem $BuildDir -Filter "*.dll" -ErrorAction SilentlyContinue | Remove-Item -Force
        Get-ChildItem $BuildDir -Filter "*.pdb" -ErrorAction SilentlyContinue | Remove-Item -Force
        Get-ChildItem $BuildDir -Filter "*.dxt" -ErrorAction SilentlyContinue | Remove-Item -Force
        Write-Host "Cleaned DXT build artifacts" -ForegroundColor Yellow
    }
    
    Write-Host "Clean completed!" -ForegroundColor Green
}

function Invoke-Restore {
    Write-Banner "Restoring NuGet packages"
    
    $ScriptDir = $PSScriptRoot
    $SourceDir = Join-Path $ScriptDir "src"
    $ProjectFile = Join-Path $SourceDir "KoikatuMCP.csproj"
    
    $dotnetPath = Find-DotNetSDK
    & $dotnetPath restore $ProjectFile --verbosity minimal
    
    if ($LASTEXITCODE -ne 0) {
        throw "Package restore failed!"
    }
    
    Write-Host "Package restore completed!" -ForegroundColor Green
}

function Invoke-Build {
    param([string]$Config)
    
    Write-Banner "Building $Config configuration"
    
    $ScriptDir = $PSScriptRoot
    $SourceDir = Join-Path $ScriptDir "src"
    $ProjectFile = Join-Path $SourceDir "KoikatuMCP.csproj"
    
    if (!(Test-Path $ProjectFile)) {
        throw "Project file not found at: $ProjectFile"
    }
    
    $dotnetPath = Find-DotNetSDK
    & $dotnetPath build $ProjectFile --configuration $Config --verbosity minimal
    
    if ($LASTEXITCODE -ne 0) {
        throw "$Config build failed!"
    }
    
    Write-Host "$Config build completed!" -ForegroundColor Green
}

function Invoke-Publish {
    param([string]$Config, [string]$Runtime, [switch]$SelfContained)
    
    Write-Banner "Publishing $Config configuration"
    
    $ScriptDir = $PSScriptRoot
    $SourceDir = Join-Path $ScriptDir "src"
    $ProjectFile = Join-Path $SourceDir "KoikatuMCP.csproj"
    
    $PublishArgs = @(
        "publish"
        $ProjectFile
        "--configuration", $Config
        "--verbosity", "minimal"
    )
    
    if ($Runtime) {
        $PublishArgs += "--runtime", $Runtime
        Write-Host "Target Runtime: $Runtime" -ForegroundColor Cyan
    }
    
    if ($SelfContained) {
        $PublishArgs += "--self-contained"
        $PublishArgs += "--property:PublishSingleFile=true"
        $PublishArgs += "--property:PublishTrimmed=false"
        Write-Host "Self-contained: Yes" -ForegroundColor Cyan
        Write-Host "Single file: Yes" -ForegroundColor Cyan
    }
    
    $dotnetPath = Find-DotNetSDK
    & $dotnetPath @PublishArgs
    
    if ($LASTEXITCODE -ne 0) {
        throw "Publish failed!"
    }
    
    Write-Host "Publish completed!" -ForegroundColor Green
}

function Invoke-Format {
    Write-Banner "Formatting code"
    
    $ScriptDir = $PSScriptRoot
    $SourceDir = Join-Path $ScriptDir "src"
    $ProjectFile = Join-Path $SourceDir "KoikatuMCP.csproj"
    
    $dotnetPath = Find-DotNetSDK
    & $dotnetPath format $ProjectFile --verbosity minimal
    
    if ($LASTEXITCODE -ne 0) {
        throw "Code formatting failed!"
    }
    
    Write-Host "Code formatting completed!" -ForegroundColor Green
}

function Invoke-BuildDxt {
    Write-Banner "Preparing DXT package"
    
    $ScriptDir = $PSScriptRoot
    $SourceDir = Join-Path $ScriptDir "src"
    $BuildDir = Join-Path $ScriptDir "build"
    $BinDir = Join-Path $SourceDir "bin"
    $ConfigDir = Join-Path $BinDir "Release"
    $Net8Dir = Join-Path $ConfigDir "net8.0"
    
    # Check for win-x64 publish directory first (single-file)  
    $WinX64Dir = Join-Path $Net8Dir "win-x64"
    $WinX64PublishDir = Join-Path $WinX64Dir "publish"
    $PublishDir = Join-Path $Net8Dir "publish"
    $ReleaseDir = $Net8Dir
    
    # Determine source directory priority: win-x64 publish > regular publish > regular build
    $SourceDir = if (Test-Path $WinX64PublishDir) { 
        Write-Host "Using win-x64 publish output (single-file)" -ForegroundColor Cyan
        $WinX64PublishDir 
    } elseif (Test-Path $PublishDir) { 
        Write-Host "Using publish output" -ForegroundColor Cyan
        $PublishDir 
    } else { 
        Write-Host "Using regular build output (multiple files)" -ForegroundColor Cyan
        $ReleaseDir 
    }
    
    # Ensure build directory exists
    if (!(Test-Path $BuildDir)) {
        New-Item -ItemType Directory -Path $BuildDir -Force | Out-Null
        Write-Host "Created build directory: $BuildDir" -ForegroundColor Gray
    }
    
    # Verify manifest exists
    $ManifestPath = Join-Path $BuildDir "manifest.json"
    if (!(Test-Path $ManifestPath)) {
        throw "manifest.json not found in build directory: $ManifestPath"
    }
    
    # Clean existing files
    Get-ChildItem $BuildDir -Filter "*.exe" -ErrorAction SilentlyContinue | Remove-Item -Force
    Get-ChildItem $BuildDir -Filter "*.dll" -ErrorAction SilentlyContinue | Remove-Item -Force
    Get-ChildItem $BuildDir -Filter "*.pdb" -ErrorAction SilentlyContinue | Remove-Item -Force
    Write-Host "Cleaned existing files in build directory" -ForegroundColor Gray
    
    # Copy executable
    $ExePath = Join-Path $SourceDir "KoikatuMCP.exe"
    if (Test-Path $ExePath) {
        Copy-Item $ExePath $BuildDir -Force
        $exeSize = [math]::Round((Get-Item $ExePath).Length / 1MB, 1)
        Write-Host "Copied: KoikatuMCP.exe ($exeSize MB)" -ForegroundColor Gray
    } else {
        throw "KoikatuMCP.exe not found at: $ExePath. Please build/publish the project first."
    }
    
    # Only copy additional files if not using single-file publish
    if ($SourceDir -eq $ReleaseDir) {
        # Copy DLL files
        $DllFiles = Get-ChildItem $SourceDir -Filter "*.dll"
        foreach ($dll in $DllFiles) {
            Copy-Item $dll.FullName $BuildDir -Force
            Write-Host "Copied: $($dll.Name)" -ForegroundColor Gray
        }
        
        # Copy runtime configuration files if they exist
        $RuntimeFiles = Get-ChildItem $SourceDir -Filter "*.json"
        foreach ($file in $RuntimeFiles) {
            Copy-Item $file.FullName $BuildDir -Force
            Write-Host "Copied: $($file.Name)" -ForegroundColor Gray
        }
    } else {
        Write-Host "Using single-file deployment - no additional DLLs needed" -ForegroundColor Green
    }
    
    Write-Host "DXT directory prepared successfully!" -ForegroundColor Green
    Write-Host "Files in build directory:" -ForegroundColor Cyan
    Get-ChildItem $BuildDir | ForEach-Object {
        $size = if ($_.PSIsContainer) { "DIR" } else { "$([math]::Round($_.Length / 1KB, 1)) KB" }
        Write-Host "  $($_.Name) ($size)" -ForegroundColor White
    }
}

function Invoke-CreateDxt {
    Write-Banner "Creating DXT package"
    
    $ScriptDir = $PSScriptRoot
    $BuildDir = Join-Path $ScriptDir "build"
    
    # Ensure build directory exists and has required files
    if (!(Test-Path $BuildDir)) {
        throw "Build directory not found: $BuildDir. Run 'build-dxt' target first."
    }
    
    $ManifestPath = Join-Path $BuildDir "manifest.json"
    if (!(Test-Path $ManifestPath)) {
        throw "manifest.json not found in build directory: $ManifestPath"
    }
    
    $ExePath = Join-Path $BuildDir "KoikatuMCP.exe"
    if (!(Test-Path $ExePath)) {
        throw "KoikatuMCP.exe not found in build directory. Run 'build-dxt' target first."
    }
    
    # Change to build directory and run dxt pack
    Push-Location $BuildDir
    try {
        # Check if dxt is installed, if not try to install it
        try {
            & npm list -g @anthropic-ai/dxt 2>$null | Out-Null
            if ($LASTEXITCODE -ne 0) {
                Write-Host "Installing @anthropic-ai/dxt..." -ForegroundColor Yellow
                & npm install -g @anthropic-ai/dxt
                if ($LASTEXITCODE -ne 0) {
                    throw "Failed to install @anthropic-ai/dxt"
                }
            }
        } catch {
            Write-Host "Installing @anthropic-ai/dxt..." -ForegroundColor Yellow
            & npm install -g @anthropic-ai/dxt
            if ($LASTEXITCODE -ne 0) {
                throw "Failed to install @anthropic-ai/dxt"
            }
        }
        
        # Try different ways to find dxt command
        $dxtCommands = @("dxt", "npx @anthropic-ai/dxt", "dxt.cmd")
        $dxtFound = $false
        
        foreach ($dxtCmd in $dxtCommands) {
            try {
                Write-Host "Trying: $dxtCmd pack" -ForegroundColor Gray
                & $dxtCmd pack
                if ($LASTEXITCODE -eq 0) {
                    $dxtFound = $true
                    break
                }
            } catch {
                continue
            }
        }
        
        if (-not $dxtFound) {
            throw "dxt command not found. Please ensure @anthropic-ai/dxt is installed: npm install -g @anthropic-ai/dxt"
        }
        
        # Find generated .dxt file
        $DxtFiles = Get-ChildItem -Filter "*.dxt"
        if ($DxtFiles.Count -gt 0) {
            Write-Host "Created DXT package: $($DxtFiles[0].Name)" -ForegroundColor Green
            Write-Host "Package size: $([math]::Round($DxtFiles[0].Length / 1KB, 1)) KB" -ForegroundColor Cyan
        } else {
            throw "DXT package was not created successfully"
        }
        
    } finally {
        Pop-Location
    }
    
    Write-Host "DXT package created successfully!" -ForegroundColor Green
}

function Show-Help {
    Write-Host "KoikatuMCP Build Script" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Usage:" -ForegroundColor Yellow
    Write-Host "  .\build.ps1 [Target] [Configuration]" -ForegroundColor White
    Write-Host ""
    Write-Host "Targets:" -ForegroundColor Yellow
    Write-Host "  clean      - Clean build outputs" -ForegroundColor White
    Write-Host "  restore    - Restore NuGet packages" -ForegroundColor White
    Write-Host "  build      - Build the solution (default)" -ForegroundColor White
    Write-Host "  rebuild    - Clean + Restore + Build" -ForegroundColor White
    Write-Host "  publish    - Publish the application" -ForegroundColor White
    Write-Host "  format     - Format code using dotnet format" -ForegroundColor White
    Write-Host "  build-dxt  - Prepare DXT package (single-file executable)" -ForegroundColor White
    Write-Host "  create-dxt - Create .dxt package file" -ForegroundColor White
    Write-Host "  dxt        - Build + Prepare + Create DXT package" -ForegroundColor White
    Write-Host ""
    Write-Host "Configurations:" -ForegroundColor Yellow
    Write-Host "  Debug      - Debug build" -ForegroundColor White
    Write-Host "  Release    - Release build (default)" -ForegroundColor White
    Write-Host ""
    Write-Host "Examples:" -ForegroundColor Yellow
    Write-Host "  .\build.ps1                    # Build Release" -ForegroundColor White
    Write-Host "  .\build.ps1 build Debug       # Build Debug" -ForegroundColor White
    Write-Host "  .\build.ps1 rebuild            # Clean + Restore + Build Release" -ForegroundColor White
    Write-Host "  .\build.ps1 format             # Format code" -ForegroundColor White
    Write-Host "  .\build.ps1 build-dxt          # Prepare single-file DXT package" -ForegroundColor White
    Write-Host "  .\build.ps1 create-dxt         # Create DXT package" -ForegroundColor White
    Write-Host "  .\build.ps1 dxt                # Full DXT build and package" -ForegroundColor White
    Write-Host "  .\build.ps1 clean              # Clean only" -ForegroundColor White
}

# Main execution logic
try {
    switch ($Target.ToLower()) {
        "help" {
            Show-Help
        }
        "clean" {
            Invoke-Clean
        }
        "restore" {
            Invoke-Restore
        }
        "build" {
            Invoke-Restore
            Invoke-Build $Configuration
            Write-Banner "Build Summary"
            $ScriptDir = $PSScriptRoot
            $SourceDir = Join-Path $ScriptDir "src"
            $BinDir = Join-Path $SourceDir "bin"
            $ConfigDir = Join-Path $BinDir $Configuration
            $ReleaseDir = Join-Path $ConfigDir "net8.0"
            if (Test-Path $ReleaseDir) {
                Write-Host "Output files:" -ForegroundColor Cyan
                
                # Show main executable
                $exeFile = Get-ChildItem $ReleaseDir -Filter "KoikatuMCP.exe" | Select-Object -First 1
                if ($exeFile) {
                    Write-Host "  $($exeFile.Name) ($([math]::Round($exeFile.Length / 1KB, 1)) KB)" -ForegroundColor White
                }
                
                # Count DLL files
                $dllCount = (Get-ChildItem $ReleaseDir -Filter "*.dll").Count
                Write-Host "  + $dllCount DLL dependencies" -ForegroundColor Gray
                
                Write-Host "Build output directory: $ReleaseDir" -ForegroundColor Cyan
            }
        }
        "rebuild" {
            Invoke-Clean
            Invoke-Restore
            Invoke-Build $Configuration
            Write-Banner "Rebuild completed successfully!"
        }
        "publish" {
            Invoke-Restore
            Invoke-Publish $Configuration
        }
        "format" {
            Invoke-Format
        }
        "build-dxt" {
            # Build single-file executable for DXT
            Invoke-Restore
            Invoke-Publish $Configuration "win-x64" -SelfContained
            Invoke-BuildDxt
        }
        "create-dxt" {
            Invoke-CreateDxt
        }
        "dxt" {
            # Full DXT workflow
            Invoke-Restore
            Invoke-Build $Configuration
            Invoke-BuildDxt
            Invoke-CreateDxt
            Write-Banner "DXT package completed successfully!"
        }
        default {
            Write-Host "Unknown target: $Target" -ForegroundColor Red
            Write-Host "Use '.\build.ps1 help' to see available options." -ForegroundColor Yellow
            exit 1
        }
    }
} catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}