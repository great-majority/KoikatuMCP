using System.ComponentModel;
using KoikatuMCP.Services;
using KoikatuMCP.Models;
using ModelContextProtocol.Server;

namespace KoikatuMCP.Tools;

[McpServerToolType]
public static class KoikatuCameraTools
{
    [McpServerTool, Description("Set the camera position, rotation, and field of view")]
    public static async Task<string> CameraSetview(
        WebSocketService webSocketService,
        [Description("Camera position [X, Y, Z] (optional)")] float[]? position = null,
        [Description("Camera rotation [pitch, yaw, roll] in degrees (optional)")] float[]? rotation = null,
        [Description("Field of view in degrees (optional)")] float? fov = null)
    {
        try
        {
            var request = new CameraCommand
            {
                type = "camera",
                command = "setview",
                pos = position,
                rot = rotation,
                fov = fov ?? 0
            };

            var response = await webSocketService.SendRequestAsync<CameraCommand, WebSocketResponse>(request);

            if (response?.Type == "success")
            {
                return $"✅ Camera view updated successfully! {response.Message}";
            }
            else
            {
                return $"❌ Failed to set camera view: {response?.Message ?? "Unknown error"}";
            }
        }
        catch (Exception ex)
        {
            return $"❌ Failed to set camera view: {ex.Message}";
        }
    }

    [McpServerTool, Description("Switch the viewport to a specific camera object")]
    public static async Task<string> CameraSwitch(
        WebSocketService webSocketService,
        [Description("ID of the camera object to switch to")] int cameraId)
    {
        try
        {
            var request = new CameraCommand
            {
                type = "camera",
                command = "switch",
                cameraId = cameraId
            };

            var response = await webSocketService.SendRequestAsync<CameraCommand, WebSocketResponse>(request);

            if (response?.Type == "success")
            {
                return $"✅ Switched to camera successfully! {response.Message}";
            }
            else
            {
                return $"❌ Failed to switch camera: {response?.Message ?? "Unknown error"}";
            }
        }
        catch (Exception ex)
        {
            return $"❌ Failed to switch camera: {ex.Message}";
        }
    }

    [McpServerTool, Description("Return to free camera mode (default)")]
    public static async Task<string> CameraFree(
        WebSocketService webSocketService)
    {
        try
        {
            var request = new CameraCommand
            {
                type = "camera",
                command = "free"
            };

            var response = await webSocketService.SendRequestAsync<CameraCommand, WebSocketResponse>(request);

            if (response?.Type == "success")
            {
                return $"✅ Switched to free camera mode! {response.Message}";
            }
            else
            {
                return $"❌ Failed to switch to free camera: {response?.Message ?? "Unknown error"}";
            }
        }
        catch (Exception ex)
        {
            return $"❌ Failed to switch to free camera: {ex.Message}";
        }
    }

    [McpServerTool, Description("Retrieve current camera information")]
    public static async Task<string> CameraGetview(
        WebSocketService webSocketService)
    {
        try
        {
            var request = new CameraCommand
            {
                type = "camera",
                command = "getview"
            };

            var response = await webSocketService.SendRequestAsync<CameraCommand, WebSocketResponse>(request);

            if (response?.Type == "success")
            {
                var pos = response.Pos ?? new float[] { 0, 0, 0 };
                var rot = response.Rot ?? new float[] { 0, 0, 0 };
                var fov = response.Fov ?? 35.0f;
                var mode = response.Mode ?? "unknown";
                var activeCameraId = response.ActiveCameraId;

                var result = $"📹 Current Camera Information:\n";
                result += $"   🎯 Position: ({pos[0]:F2}, {pos[1]:F2}, {pos[2]:F2})\n";
                result += $"   🔄 Rotation: ({rot[0]:F2}, {rot[1]:F2}, {rot[2]:F2})\n";
                result += $"   🔍 Field of View: {fov:F1}°\n";
                result += $"   📷 Mode: {mode}\n";

                if (activeCameraId.HasValue)
                {
                    result += $"   🎬 Active Camera ID: {activeCameraId.Value}";
                }
                else
                {
                    result += $"   🎬 Active Camera: Free Camera";
                }

                return result;
            }
            else
            {
                return $"❌ Failed to get camera view: {response?.Message ?? "Unknown error"}";
            }
        }
        catch (Exception ex)
        {
            return $"❌ Failed to get camera view: {ex.Message}";
        }
    }
}