using System.ComponentModel;
using System.Text.Json;
using KoikatuMCP.Services;
using KoikatuMCP.Models;
using ModelContextProtocol.Server;
using Microsoft.Extensions.AI;

namespace KoikatuMCP.Tools;

[McpServerToolType]
public static class KoikatuScreenshotTools
{
    [McpServerTool, Description("Capture the current Studio view as a PNG image")]
    public static async Task<AIContent> Screenshot(
        WebSocketService webSocketService,
        [Description("Image width in pixels (default: 854)")] int? width = null,
        [Description("Image height in pixels (default: 480)")] int? height = null,
        [Description("Include alpha channel for transparency (default: false)")] bool? transparency = null,
        [Description("Include capture mark overlay (default: true)")] bool? mark = null)
    {
        try
        {
            var request = new ScreenshotCommand
            {
                type = "screenshot",
                width = width,
                height = height,
                transparency = transparency,
                mark = mark
            };

            var response = await webSocketService.SendRequestAsync<ScreenshotCommand, WebSocketResponse>(request);

            if (response?.Type == "success")
            {
                var screenshotData = response.Data;
                if (screenshotData != null)
                {
                    var data = JsonSerializer.Deserialize<ScreenshotData>(screenshotData.ToString()!);
                    if (data != null)
                    {
                        // Return the image data directly as DataContent
                        return new DataContent($"data:image/{data.Format.ToLower()};base64,{data.Image}");
                    }
                }

                return new TextContent($"✅ Screenshot taken successfully! {response.Message}");
            }
            else
            {
                return new TextContent($"❌ Failed to take screenshot: {response?.Message ?? "Unknown error"}");
            }
        }
        catch (Exception ex)
        {
            return new TextContent($"❌ Failed to take screenshot: {ex.Message}");
        }
    }

    private static string FormatFileSize(int bytes)
    {
        if (bytes < 1024)
            return $"{bytes} B";
        else if (bytes < 1024 * 1024)
            return $"{bytes / 1024.0:F1} KB";
        else
            return $"{bytes / (1024.0 * 1024.0):F1} MB";
    }
}