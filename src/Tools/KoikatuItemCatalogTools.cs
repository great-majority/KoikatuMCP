using System.ComponentModel;
using System.Text.Json;
using KoikatuMCP.Services;
using KoikatuMCP.Models;
using ModelContextProtocol.Server;

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

            var responseJson = await webSocketService.SendRequestAsync<ItemCommand, string>(request);

            if (string.IsNullOrEmpty(responseJson))
            {
                return "❌ Failed to get item groups: Empty response";
            }

            var response = JsonSerializer.Deserialize<ItemCatalogResponse>(responseJson);

            if (response?.Type == "success" && response.Data != null)
            {
                var groups = JsonSerializer.Deserialize<List<ItemGroup>>(response.Data.ToString()!);

                if (groups == null || groups.Count == 0)
                {
                    return "📭 No item groups found";
                }

                var result = "📦 Available Item Groups:\n";
                foreach (var group in groups)
                {
                    result += $"   🏷️ Group {group.Id}: {group.Name} ({group.CategoryCount} categories)\n";
                }

                return result;
            }
            else
            {
                return $"❌ Failed to get item groups: {response?.Data?.ToString() ?? "Unknown error"}";
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

            var responseJson = await webSocketService.SendRequestAsync<ItemCommand, string>(request);

            if (string.IsNullOrEmpty(responseJson))
            {
                return "❌ Failed to get item categories: Empty response";
            }

            var response = JsonSerializer.Deserialize<ItemCatalogResponse>(responseJson);

            if (response?.Type == "success" && response.Data != null)
            {
                var groupData = JsonSerializer.Deserialize<ItemGroup>(response.Data.ToString()!);

                if (groupData?.Categories == null || groupData.Categories.Count == 0)
                {
                    return $"📭 No categories found in group {groupId}";
                }

                var result = $"📁 Categories in Group {groupId} ({groupData.Name}):\n";
                foreach (var category in groupData.Categories)
                {
                    result += $"   📂 Category {category.Id}: {category.Name} ({category.ItemCount} items)\n";
                }

                return result;
            }
            else
            {
                return $"❌ Failed to get item categories: {response?.Data?.ToString() ?? "Unknown error"}";
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

            var responseJson = await webSocketService.SendRequestAsync<ItemCommand, string>(request);

            if (string.IsNullOrEmpty(responseJson))
            {
                return "❌ Failed to get category items: Empty response";
            }

            var response = JsonSerializer.Deserialize<ItemCatalogResponse>(responseJson);

            if (response?.Type == "success" && response.Data != null)
            {
                var categoryData = JsonSerializer.Deserialize<ItemCategory>(response.Data.ToString()!);

                if (categoryData?.Items == null || categoryData.Items.Count == 0)
                {
                    return $"📭 No items found in category {categoryId} of group {groupId}";
                }

                var result = $"🔧 Items in Group {groupId}, Category {categoryId} ({categoryData.Name}):\n";
                foreach (var item in categoryData.Items)
                {
                    result += $"   🎯 Item {item.Id}: {item.Name}\n";

                    if (item.Properties != null)
                    {
                        var props = item.Properties;
                        result += $"      • Colors: {props.ColorSlots}, Patterns: {props.PatternSlots}\n";
                        result += $"      • Scale: {props.IsScale}, Anime: {props.IsAnime}\n";
                        result += $"      • Glass: {props.IsGlass}, Emission: {props.IsEmission}\n";
                    }
                }

                return result;
            }
            else
            {
                return $"❌ Failed to get category items: {response?.Data?.ToString() ?? "Unknown error"}";
            }
        }
        catch (Exception ex)
        {
            return $"❌ Failed to get category items: {ex.Message}";
        }
    }
}