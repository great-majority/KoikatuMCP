using System.ComponentModel;
using System.Text.Json;
using KoikatuMCP.Models;
using KoikatuMCP.Services;
using ModelContextProtocol.Server;

namespace KoikatuMCP.Tools;

[McpServerToolType]
public static class KoikatuStudioTools
{
    [McpServerTool, Description("Test connection to KKStudioSocket WebSocket server")]
    public static async Task<string> Ping(
        WebSocketService webSocketService,
        [Description("Message to send with ping")] string message = "test")
    {
        try
        {
            var request = new PingCommand
            {
                type = "ping",
                message = message,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            var response = await webSocketService.SendRequestAsync<PingCommand, WebSocketResponse>(request);

            if (response?.Type == "pong")
            {
                return $"✅ Ping successful! Server responded with: {response.Message}";
            }
            else
            {
                return $"❌ Ping failed. Unexpected response: {response?.Type}";
            }
        }
        catch (Exception ex)
        {
            return $"❌ Ping failed: {ex.Message}";
        }
    }

    [McpServerTool, Description("Get the hierarchical structure of objects in the scene")]
    public static async Task<string> Tree(
        WebSocketService webSocketService,
        [Description("Maximum depth to retrieve (optional)")] int? depth = null,
        [Description("Specific object ID to get subtree from (optional)")] int? objectId = null)
    {
        try
        {
            var request = new TreeCommand
            {
                type = "tree",
                depth = depth,
                id = objectId
            };

            var responseJson = await webSocketService.SendRequestAsync<TreeCommand, string>(request);

            if (string.IsNullOrEmpty(responseJson))
            {
                return "❌ Failed to get scene tree: Empty response";
            }

            // Parse the response to validate it's a valid scene tree
            var sceneObjects = JsonSerializer.Deserialize<List<SceneObject>>(responseJson);

            if (sceneObjects == null || sceneObjects.Count == 0)
            {
                return "📭 Scene is empty - no objects found";
            }

            // Format the response for better readability
            var formattedResponse = FormatSceneTree(sceneObjects, 0);
            return $"🌲 Scene Tree:\n{formattedResponse}";

        }
        catch (Exception ex)
        {
            return $"❌ Failed to get scene tree: {ex.Message}";
        }
    }

    private static string FormatSceneTree(List<SceneObject> objects, int indent)
    {
        var result = new System.Text.StringBuilder();
        var indentStr = new string(' ', indent * 2);

        foreach (var obj in objects)
        {
            result.AppendLine($"{indentStr}📦 {obj.Name} (ID: {obj.ObjectInfo.Id}, Type: {obj.ObjectInfo.Type})");

            if (obj.ObjectInfo.Transform != null)
            {
                var pos = obj.ObjectInfo.Transform.Pos;
                var rot = obj.ObjectInfo.Transform.Rot;
                var scale = obj.ObjectInfo.Transform.Scale;
                result.AppendLine($"{indentStr}   🎯 Position: ({pos[0]:F2}, {pos[1]:F2}, {pos[2]:F2})");
                result.AppendLine($"{indentStr}   🔄 Rotation: ({rot[0]:F2}, {rot[1]:F2}, {rot[2]:F2})");
                result.AppendLine($"{indentStr}   📏 Scale: ({scale[0]:F2}, {scale[1]:F2}, {scale[2]:F2})");
            }

            if (obj.ObjectInfo.ItemDetail != null)
            {
                var detail = obj.ObjectInfo.ItemDetail;
                result.AppendLine($"{indentStr}   📋 Item Detail: Group={detail.Group}, Category={detail.Category}, ItemId={detail.ItemId}");
            }

            if (obj.Children.Count > 0)
            {
                result.AppendLine($"{indentStr}   📁 Children ({obj.Children.Count}):");
                result.Append(FormatSceneTree(obj.Children, indent + 2));
            }
        }

        return result.ToString();
    }

    [McpServerTool, Description("Add an item to the scene")]
    public static async Task<string> AddItem(
        WebSocketService webSocketService,
        [Description("Item group ID")] int group,
        [Description("Item category ID")] int category,
        [Description("Item ID within the category")] int itemId)
    {
        try
        {
            var request = new AddCommand
            {
                type = "add",
                command = "item",
                group = group,
                category = category,
                itemId = itemId
            };

            var response = await webSocketService.SendRequestAsync<AddCommand, WebSocketResponse>(request);

            if (response?.Type == "success")
            {
                return $"✅ Item added successfully! Object ID: {response.ObjectId}. {response.Message}";
            }
            else
            {
                return $"❌ Failed to add item: {response?.Message ?? "Unknown error"}";
            }
        }
        catch (Exception ex)
        {
            return $"❌ Failed to add item: {ex.Message}";
        }
    }

    [McpServerTool, Description("Add a light to the scene")]
    public static async Task<string> AddLight(
        WebSocketService webSocketService,
        [Description("Light type ID (0=Directional, 1=Point, 2=Spot)")] int lightId)
    {
        try
        {
            var request = new AddCommand
            {
                type = "add",
                command = "light",
                lightId = lightId
            };

            var response = await webSocketService.SendRequestAsync<AddCommand, WebSocketResponse>(request);

            if (response?.Type == "success")
            {
                return $"✅ Light added successfully! Object ID: {response.ObjectId}. {response.Message}";
            }
            else
            {
                return $"❌ Failed to add light: {response?.Message ?? "Unknown error"}";
            }
        }
        catch (Exception ex)
        {
            return $"❌ Failed to add light: {ex.Message}";
        }
    }

    [McpServerTool, Description("Update position, rotation, or scale of an object")]
    public static async Task<string> UpdateTransform(
        WebSocketService webSocketService,
        [Description("Object ID to update")] int objectId,
        [Description("Position [X, Y, Z] (optional)")] float[]? position = null,
        [Description("Rotation [X, Y, Z] in degrees (optional)")] float[]? rotation = null,
        [Description("Scale [X, Y, Z] (optional)")] float[]? scale = null)
    {
        try
        {
            var request = new UpdateCommand
            {
                type = "update",
                command = "transform",
                id = objectId,
                pos = position,
                rot = rotation,
                scale = scale
            };

            var response = await webSocketService.SendRequestAsync<UpdateCommand, WebSocketResponse>(request);

            if (response?.Type == "success")
            {
                return $"✅ Transform updated successfully! {response.Message}";
            }
            else
            {
                return $"❌ Failed to update transform: {response?.Message ?? "Unknown error"}";
            }
        }
        catch (Exception ex)
        {
            return $"❌ Failed to update transform: {ex.Message}";
        }
    }

    [McpServerTool, Description("Delete an object from the scene")]
    public static async Task<string> Delete(
        WebSocketService webSocketService,
        [Description("ID of the object to delete")] int objectId)
    {
        try
        {
            var request = new DeleteCommand
            {
                type = "delete",
                id = objectId
            };

            var response = await webSocketService.SendRequestAsync<DeleteCommand, WebSocketResponse>(request);

            if (response?.Type == "success")
            {
                return $"✅ Object deleted successfully! {response.Message}";
            }
            else
            {
                return $"❌ Failed to delete object: {response?.Message ?? "Unknown error"}";
            }
        }
        catch (Exception ex)
        {
            return $"❌ Failed to delete object: {ex.Message}";
        }
    }
}