using System.ComponentModel;
using KoikatuMCP.Services;
using ModelContextProtocol.Server;
using KKStudioSocket.Models.Requests;
using KKStudioSocket.Models.Responses;

namespace KoikatuMCP.Tools;

[McpServerToolType]
public static class KoikatuHierarchyTools
{
    [McpServerTool, Description("Parent an object to another object in the scene hierarchy")]
    public static async Task<string> HierarchyAttach(
        WebSocketService webSocketService,
        [Description("ID of the child object to attach")] int childId,
        [Description("ID of the parent object")] int parentId)
    {
        try
        {
            var request = new HierarchyCommand
            {
                type = "hierarchy",
                command = "attach",
                childId = childId,
                parentId = parentId
            };

            var response = await webSocketService.SendRequestAsync<HierarchyCommand, SuccessResponse>(request);

            if (response?.type == "success")
            {
                return $"✅ Hierarchy updated successfully! Child {childId} attached to parent {parentId}. {response.message}";
            }
            else
            {
                return $"❌ Failed to attach object: {response?.message ?? "Unknown error"}";
            }
        }
        catch (Exception ex)
        {
            return $"❌ Failed to attach object: {ex.Message}";
        }
    }

    [McpServerTool, Description("Detach an object from its parent in the scene hierarchy (make it a root object)")]
    public static async Task<string> HierarchyDetach(
        WebSocketService webSocketService,
        [Description("ID of the child object to detach from its parent")] int childId)
    {
        try
        {
            var request = new HierarchyCommand
            {
                type = "hierarchy",
                command = "detach",
                childId = childId
            };

            var response = await webSocketService.SendRequestAsync<HierarchyCommand, SuccessResponse>(request);

            if (response?.type == "success")
            {
                return $"✅ Hierarchy updated successfully! Object {childId} detached from parent. {response.message}";
            }
            else
            {
                return $"❌ Failed to detach object: {response?.message ?? "Unknown error"}";
            }
        }
        catch (Exception ex)
        {
            return $"❌ Failed to detach object: {ex.Message}";
        }
    }
}