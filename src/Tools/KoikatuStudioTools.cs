using System.ComponentModel;
using KoikatuMCP.Services;
using ModelContextProtocol.Server;
using KKStudioSocket.Models.Requests;
using KKStudioSocket.Models.Responses;

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

            var response = await webSocketService.SendRequestAsync<PingCommand, PongResponse>(request);

            if (response?.type == "pong")
            {
                return $"✅ Ping successful! Server responded with: {response.message}";
            }
            else
            {
                return $"❌ Ping failed. Unexpected response: {response?.type}";
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
        [Description("Maximum depth to retrieve (default: 1)")] int? depth = 1,
        [Description("Specific object ID to get subtree from (optional). If not specified, retrieves all root objects in the scene")] int? objectId = null)
    {
        try
        {
            var request = new TreeCommand
            {
                type = "tree",
                depth = depth,
                id = objectId
            };

            var response = await webSocketService.SendRequestAsync<TreeCommand, TreeResponse>(request);

            if (response?.data == null || response.data.Count == 0)
            {
                return "📭 Scene is empty - no objects found";
            }

            // Format the response for better readability
            var formattedResponse = FormatSceneTree(response.data, 0);
            return $"🌲 Scene Tree:\n{formattedResponse}";

        }
        catch (Exception ex)
        {
            return $"❌ Failed to get scene tree: {ex.Message}";
        }
    }

    private static string FormatSceneTree(List<TreeNode> objects, int indent)
    {
        var result = new System.Text.StringBuilder();
        var indentStr = new string(' ', indent * 2);

        foreach (var obj in objects)
        {
            result.AppendLine($"{indentStr}📦 {obj.name} (ID: {obj.objectInfo.id}, Type: {obj.objectInfo.type})");

            if (obj.objectInfo.transform != null)
            {
                var pos = obj.objectInfo.transform.pos;
                var rot = obj.objectInfo.transform.rot;
                var scale = obj.objectInfo.transform.scale;
                result.AppendLine($"{indentStr}   🎯 Position: ({pos[0]:F2}, {pos[1]:F2}, {pos[2]:F2})");
                result.AppendLine($"{indentStr}   🔄 Rotation: ({rot[0]:F2}, {rot[1]:F2}, {rot[2]:F2})");
                result.AppendLine($"{indentStr}   📏 Scale: ({scale[0]:F2}, {scale[1]:F2}, {scale[2]:F2})");
            }

            if (obj.objectInfo.itemDetail != null)
            {
                var detail = obj.objectInfo.itemDetail;
                result.AppendLine($"{indentStr}   📋 Item Detail: Group={detail.group}, Category={detail.category}, ItemId={detail.itemId}");
            }

            if (obj.children.Count > 0)
            {
                result.AppendLine($"{indentStr}   📁 Children ({obj.children.Count}):");
                result.Append(FormatSceneTree(obj.children, indent + 2));
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

            var response = await webSocketService.SendRequestAsync<AddCommand, AddSuccessResponse>(request);

            if (response?.type == "success")
            {
                return $"✅ Item added successfully! Object ID: {response.objectId}. {response.message}";
            }
            else
            {
                return $"❌ Failed to add item: {response?.message ?? "Unknown error"}";
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

            var response = await webSocketService.SendRequestAsync<AddCommand, AddSuccessResponse>(request);

            if (response?.type == "success")
            {
                return $"✅ Light added successfully! Object ID: {response.objectId}. {response.message}";
            }
            else
            {
                return $"❌ Failed to add light: {response?.message ?? "Unknown error"}";
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

            var response = await webSocketService.SendRequestAsync<UpdateCommand, SuccessResponse>(request);

            if (response?.type == "success")
            {
                return $"✅ Transform updated successfully! {response.message}";
            }
            else
            {
                return $"❌ Failed to update transform: {response?.message ?? "Unknown error"}";
            }
        }
        catch (Exception ex)
        {
            return $"❌ Failed to update transform: {ex.Message}";
        }
    }

    [McpServerTool, Description("Add a character to the scene")]
    public static async Task<string> AddCharacter(
        WebSocketService webSocketService,
        [Description("Path to the character file")] string path,
        [Description("Character sex (male/female)")] string sex)
    {
        try
        {
            var request = new AddCommand
            {
                type = "add",
                command = "character",
                path = path,
                sex = sex
            };

            var response = await webSocketService.SendRequestAsync<AddCommand, AddSuccessResponse>(request);

            if (response?.type == "success")
            {
                return $"✅ Character added successfully! Object ID: {response.objectId}. {response.message}";
            }
            else
            {
                return $"❌ Failed to add character: {response?.message ?? "Unknown error"}";
            }
        }
        catch (Exception ex)
        {
            return $"❌ Failed to add character: {ex.Message}";
        }
    }

    [McpServerTool, Description("Add an organizational folder to the scene")]
    public static async Task<string> AddFolder(
        WebSocketService webSocketService,
        [Description("Name of the folder")] string name)
    {
        try
        {
            var request = new AddCommand
            {
                type = "add",
                command = "folder",
                name = name
            };

            var response = await webSocketService.SendRequestAsync<AddCommand, AddSuccessResponse>(request);

            if (response?.type == "success")
            {
                return $"✅ Folder added successfully! Object ID: {response.objectId}. {response.message}";
            }
            else
            {
                return $"❌ Failed to add folder: {response?.message ?? "Unknown error"}";
            }
        }
        catch (Exception ex)
        {
            return $"❌ Failed to add folder: {ex.Message}";
        }
    }

    [McpServerTool, Description("Add a camera object to the scene")]
    public static async Task<string> AddCamera(
        WebSocketService webSocketService,
        [Description("Name of the camera")] string name)
    {
        try
        {
            var request = new AddCommand
            {
                type = "add",
                command = "camera",
                name = name
            };

            var response = await webSocketService.SendRequestAsync<AddCommand, AddSuccessResponse>(request);

            if (response?.type == "success")
            {
                return $"✅ Camera added successfully! Object ID: {response.objectId}. {response.message}";
            }
            else
            {
                return $"❌ Failed to add camera: {response?.message ?? "Unknown error"}";
            }
        }
        catch (Exception ex)
        {
            return $"❌ Failed to add camera: {ex.Message}";
        }
    }

    [McpServerTool, Description("Update the color or material properties of an object")]
    public static async Task<string> UpdateColor(
        WebSocketService webSocketService,
        [Description("Object ID to update")] int objectId,
        [Description("Color values [R, G, B, A] (0.0-1.0) (optional)")] float[]? color = null)
    {
        try
        {
            var request = new UpdateCommand
            {
                type = "update",
                command = "color",
                id = objectId,
                color = color
            };

            var response = await webSocketService.SendRequestAsync<UpdateCommand, SuccessResponse>(request);

            if (response?.type == "success")
            {
                return $"✅ Color updated successfully! {response.message}";
            }
            else
            {
                return $"❌ Failed to update color: {response?.message ?? "Unknown error"}";
            }
        }
        catch (Exception ex)
        {
            return $"❌ Failed to update color: {ex.Message}";
        }
    }

    [McpServerTool, Description("Update the visibility of an object")]
    public static async Task<string> UpdateVisibility(
        WebSocketService webSocketService,
        [Description("Object ID to update")] int objectId,
        [Description("Visibility state (true=visible, false=hidden)")] bool visible)
    {
        try
        {
            var request = new UpdateCommand
            {
                type = "update",
                command = "visibility",
                id = objectId,
                visible = visible
            };

            var response = await webSocketService.SendRequestAsync<UpdateCommand, SuccessResponse>(request);

            if (response?.type == "success")
            {
                return $"✅ Visibility updated successfully! Object {(visible ? "shown" : "hidden")}. {response.message}";
            }
            else
            {
                return $"❌ Failed to update visibility: {response?.message ?? "Unknown error"}";
            }
        }
        catch (Exception ex)
        {
            return $"❌ Failed to update visibility: {ex.Message}";
        }
    }

    [McpServerTool, Description("Update light-specific properties")]
    public static async Task<string> UpdateLight(
        WebSocketService webSocketService,
        [Description("Light object ID to update")] int objectId,
        [Description("Light intensity (optional)")] float? intensity = null,
        [Description("Light range (optional)")] float? range = null,
        [Description("Spot light angle in degrees (optional)")] float? spotAngle = null,
        [Description("Enable/disable light (optional)")] bool? enable = null)
    {
        try
        {
            var request = new UpdateCommand
            {
                type = "update",
                command = "light",
                id = objectId,
                intensity = intensity,
                range = range,
                spotAngle = spotAngle,
                enable = enable
            };

            var response = await webSocketService.SendRequestAsync<UpdateCommand, SuccessResponse>(request);

            if (response?.type == "success")
            {
                return $"✅ Light properties updated successfully! {response.message}";
            }
            else
            {
                return $"❌ Failed to update light: {response?.message ?? "Unknown error"}";
            }
        }
        catch (Exception ex)
        {
            return $"❌ Failed to update light: {ex.Message}";
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

            var response = await webSocketService.SendRequestAsync<DeleteCommand, SuccessResponse>(request);

            if (response?.type == "success")
            {
                return $"✅ Object deleted successfully! {response.message}";
            }
            else
            {
                return $"❌ Failed to delete object: {response?.message ?? "Unknown error"}";
            }
        }
        catch (Exception ex)
        {
            return $"❌ Failed to delete object: {ex.Message}";
        }
    }
}