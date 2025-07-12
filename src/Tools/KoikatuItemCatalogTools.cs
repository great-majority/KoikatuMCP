using System.ComponentModel;
using KoikatuMCP.Services;
using ModelContextProtocol.Server;
using KKStudioSocket.Models.Requests;
using KKStudioSocket.Models.Responses;

namespace KoikatuMCP.Tools;

[McpServerToolType]
public static class KoikatuItemCatalogTools
{
    [McpServerTool, Description("Get a list of all item groups")]
    public static async Task<string> ItemListGroups(
        WebSocketService webSocketService)
    {
        try
        {
            var request = new ItemCommand
            {
                type = "item",
                command = "list-groups"
            };

            var response = await webSocketService.SendRequestAsync<ItemCommand, ItemGroupsResponse>(request);

            if (response?.type == "success" && response.data != null)
            {
                if (response.data.Count == 0)
                {
                    return "📭 No item groups found";
                }

                var result = "📦 Available Item Groups:\n";
                foreach (var group in response.data)
                {
                    result += $"   🏷️ Group {group.id}: {group.name} ({group.categoryCount} categories)\n";
                }

                return result;
            }
            else
            {
                return $"❌ Failed to get item groups: {response?.message ?? "Unknown error"}";
            }
        }
        catch (Exception ex)
        {
            return $"❌ Failed to get item groups: {ex.Message}";
        }
    }

    [McpServerTool, Description("Get categories within a specific group")]
    public static async Task<string> ItemListGroup(
        WebSocketService webSocketService,
        [Description("Item group ID")] int groupId)
    {
        try
        {
            var request = new ItemCommand
            {
                type = "item",
                command = "list-group",
                groupId = groupId
            };

            var response = await webSocketService.SendRequestAsync<ItemCommand, ItemGroupDetailResponse>(request);

            if (response?.type == "success" && response.data != null)
            {
                if (response.data.categories == null || response.data.categories.Count == 0)
                {
                    return $"📭 No categories found in group {groupId}";
                }

                var result = $"📁 Categories in Group {groupId} ({response.data.name}):\n";
                foreach (var category in response.data.categories)
                {
                    result += $"   📂 Category {category.id}: {category.name} ({category.itemCount} items)\n";
                }

                return result;
            }
            else
            {
                return $"❌ Failed to get item categories: {response?.message ?? "Unknown error"}";
            }
        }
        catch (Exception ex)
        {
            return $"❌ Failed to get item categories: {ex.Message}";
        }
    }

    [McpServerTool, Description("Get all items within a specific category")]
    public static async Task<string> ItemListCategory(
        WebSocketService webSocketService,
        [Description("Item group ID")] int groupId,
        [Description("Item category ID")] int categoryId)
    {
        try
        {
            var request = new ItemCommand
            {
                type = "item",
                command = "list-category",
                groupId = groupId,
                categoryId = categoryId
            };

            var response = await webSocketService.SendRequestAsync<ItemCommand, ItemCategoryDetailResponse>(request);

            if (response?.type == "success" && response.data != null)
            {
                if (response.data.items == null || response.data.items.Count == 0)
                {
                    return $"📭 No items found in category {categoryId} of group {groupId}";
                }

                var result = $"🔧 Items in Group {groupId}, Category {categoryId} ({response.data.name}):\n";
                foreach (var item in response.data.items)
                {
                    result += $"   🎯 Item {item.id}: {item.name}\n";

                    if (item.properties != null)
                    {
                        var props = item.properties;
                        result += $"      • Colors: {props.colorSlots}, Patterns: {props.patternSlots}\n";
                        result += $"      • Scale: {props.isScale}, Anime: {props.isAnime}\n";
                        result += $"      • Glass: {props.isGlass}, Emission: {props.isEmission}\n";
                    }
                }

                return result;
            }
            else
            {
                return $"❌ Failed to get category items: {response?.message ?? "Unknown error"}";
            }
        }
        catch (Exception ex)
        {
            return $"❌ Failed to get category items: {ex.Message}";
        }
    }
}