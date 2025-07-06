using System.ComponentModel;
using System.Text.Json;
using KoikatuMCP.Services;
using KoikatuMCP.Models;
using ModelContextProtocol.Server;

namespace KoikatuMCP.Tools;

[McpServerToolType]
public static class KoikatuScreenshotTools
{
    [McpServerTool, Description("Capture the current Studio view as a PNG image")]
    public static async Task<string> Screenshot(
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
                        var result = $"📸 Screenshot captured successfully!\n";
                        result += $"   📏 Size: {data.Width} x {data.Height} pixels\n";
                        result += $"   📁 Format: {data.Format.ToUpper()}\n";
                        result += $"   💾 File Size: {FormatFileSize(data.Size)}\n";
                        result += $"   🌈 Transparency: {(data.Transparency ? "Yes" : "No")}\n";
                        result += $"   🖼️ Base64 Image Data: {data.Image.Length} characters\n";
                        result += $"\nTo use this image:\n";
                        result += $"1. Copy the base64 data from the 'image' field\n";
                        result += $"2. Decode it to save as a PNG file\n";
                        result += $"3. Or use it directly in HTML: <img src=\"data:image/png;base64,{data.Image.Substring(0, Math.Min(50, data.Image.Length))}...\" />";

                        return result;
                    }
                }

                return $"✅ Screenshot taken successfully! {response.Message}";
            }
            else
            {
                return $"❌ Failed to take screenshot: {response?.Message ?? "Unknown error"}";
            }
        }
        catch (Exception ex)
        {
            return $"❌ Failed to take screenshot: {ex.Message}";
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