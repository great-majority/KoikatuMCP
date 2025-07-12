using System.ComponentModel;
using KoikatuMCP.Services;
using ModelContextProtocol.Server;
using KKStudioSocket.Models.Requests;
using KKStudioSocket.Models.Responses;

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
                fov = fov
            };

            var response = await webSocketService.SendRequestAsync<CameraCommand, SuccessResponse>(request);

            if (response?.type == "success")
            {
                return $"✅ Camera view updated successfully! {response.message}";
            }
            else
            {
                return $"❌ Failed to set camera view: {response?.message ?? "Unknown error"}";
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

            var response = await webSocketService.SendRequestAsync<CameraCommand, SuccessResponse>(request);

            if (response?.type == "success")
            {
                return $"✅ Switched to camera successfully! {response.message}";
            }
            else
            {
                return $"❌ Failed to switch camera: {response?.message ?? "Unknown error"}";
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

            var response = await webSocketService.SendRequestAsync<CameraCommand, SuccessResponse>(request);

            if (response?.type == "success")
            {
                return $"✅ Switched to free camera mode! {response.message}";
            }
            else
            {
                return $"❌ Failed to switch to free camera: {response?.message ?? "Unknown error"}";
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

            var response = await webSocketService.SendRequestAsync<CameraCommand, CameraViewResponse>(request);

            if (response?.type == "success")
            {
                var pos = response.pos ?? new float[] { 0, 0, 0 };
                var rot = response.rot ?? new float[] { 0, 0, 0 };
                var fov = response.fov;
                var mode = response.mode ?? "unknown";
                var activeCameraId = response.activeCameraId;

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
                return $"❌ Failed to get camera view: {response?.message ?? "Unknown error"}";
            }
        }
        catch (Exception ex)
        {
            return $"❌ Failed to get camera view: {ex.Message}";
        }
    }
}