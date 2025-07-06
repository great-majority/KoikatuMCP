# KoikatuMCP

[日本語](README.ja.md) | English

[![Build and Test](https://github.com/great-majority/KoikatuMCP/actions/workflows/ci.yml/badge.svg)](https://github.com/great-majority/KoikatuMCP/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

A Model Context Protocol (MCP) server that enables AI assistants to control Koikatsu Studio through natural language commands.

## Requirements

- **Koikatsu (KK)** or **Koikatsu Sunshine (KKS)**
- **[KKStudioSocket](https://github.com/great-majority/KKStudioSocket)** plugin
- **LLM systems** that operate as MCP clients, such as Claude

## Installation

### 1. Install KKStudioSocket Plugin
Follow the [KKStudioSocket installation guide](https://github.com/great-majority/KKStudioSocket?tab=readme-ov-file#-installation) to set up the WebSocket plugin.

### 2. Download KoikatuMCP
1. Download the latest release from [Releases](https://github.com/great-majority/KoikatuMCP/releases)
2. Extract the ZIP file to any location
3. The archive contains:
   - `KoikatuMCP.exe` (single executable file)
   - `README.md` (this file)
   - `LICENSE` (license information)

## Setup with Claude Desktop

Add the following configuration to your Claude Desktop MCP settings:

### Via Claude Desktop Settings
1. Locate the path to `KoikatuMCP.exe` (Right-click the `KoikatuMCP.exe` file inside the unzipped folder → Select "Copy as path")
2. Open Claude Desktop
3. Go to **Settings** → **Developer** → **Edit Config**
4. A File Explorer window will open at `%APPDATA%\Claude`. Open `claude_desktop_config.json` in a text editor.
5. Add the following entry and save file:

```json
{
  "mcpServers": {
    "koikatu-mcp": {
      "command": "Paste the full path to KoikatuMCP.exe here",
      "env": {
        "KKSTUDIOSOCKET_URL": "ws://127.0.0.1:8765/ws"
      }
    }
  }
}
```

⚠️ Note: Backslashes in the path must be escaped by doubling them.
Example:
```
❌ Incorrect: "C:\Users\user\Downloads\KoikatuMCP.exe"  
✅ Correct: "C:\\Users\\user\\Downloads\\KoikatuMCP.exe"
```

### Via Configuration File (e.g., Claude Code, Gemini CLI)
Add to your `claude_desktop_config.json`:
```json
{
  "mcpServers": {
    "koikatu-mcp": {
      "command": "C:\\path\\to\\KoikatuMCP.exe"
    }
  }
}
```

## Usage

### 1. Start Koikatsu Studio
1. Launch Koikatsu or Koikatsu Sunshine Studio
2. Ensure KKStudioSocket is active (WebSocket server running on port 8765)


You can verify whether KKStudioSocket is loaded correctly by checking for the following lines in `output_log.txt`:
```
[Info   :   BepInEx] Loading [KKStudioSocket 1.0.0]
[Info   :KKStudioSocket] Awaking KKStudioSocket v1.0.0
```

### 2. Start Claude Desktop
With the MCP configuration in place, KoikatuMCP will automatically connect when you start a conversation.

### 3. Talk with yout LLM Chatbot

#### Connection Test

```
> Please run a ping to check the connection!

● Executing connection check to KKStudioSocket!

● `koikatu-mcp:ping` (MCP) (message: "Connection test")
  ⎿  ✅ Ping successful! Server responded with: Connection test

● ✅ Connection successful!

  The connection to KKStudioSocket has been successfully established.  
  The WebSocket server is functioning correctly, and KoikatuMCP is now able to communicate with Koikatsu Studio.

  You can now use all available features, including scene manipulation, item placement, and camera control!
```

#### Inspect Items Placed in the Scene

```
> What items are currently placed in the scene?

● To check which items are placed in the current scene, let's retrieve the scene tree.

● `koikatu-mcp:tree` (MCP) (depth: 2)  
  ⎿  🌲 Scene Tree:  
     📦 Folder (ID: 127, Type: OCIFolder)  
        🎯 Position: (3.79, -1.54, -16.95)  
     … +821 lines (ctrl+r to expand)

● The following items are currently placed in the scene:

  **Main Objects**

  1. **Character**

  - Haruka Shinozaki (ID: 0) – Female character  
    - Position: (0.00, 0.00, 0.00)

  2. **Item**

...
```

## Available Commands

### Scene Management
- Add objects (items, lights, cameras, characters)
- Modify object properties (position, rotation, scale, color)
- Remove objects from the scene
- Get scene hierarchy and structure

### Camera Control
- Position and orient the camera
- Switch between camera views
- Capture screenshots

### Content Discovery
- Browse available item catalogs
- Search for specific items
- Get detailed item information

### Object Manipulation
- Attach/detach objects to create hierarchies
- Modify visibility and appearance
- Adjust lighting properties

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Links

- [KKStudioSocket Plugin](https://github.com/great-majority/KKStudioSocket)
- [Model Context Protocol](https://modelcontextprotocol.io/)
- [Contributing Guide](CONTRIBUTING.md)