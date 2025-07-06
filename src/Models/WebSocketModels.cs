using System.Text.Json.Serialization;

namespace KoikatuMCP.Models;

public class WebSocketResponse
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("timestamp")]
    public long? Timestamp { get; set; }

    [JsonPropertyName("objectId")]
    public int? ObjectId { get; set; }

    [JsonPropertyName("command")]
    public string? Command { get; set; }

    [JsonPropertyName("groupId")]
    public int? GroupId { get; set; }

    [JsonPropertyName("categoryId")]
    public int? CategoryId { get; set; }

    [JsonPropertyName("data")]
    public object? Data { get; set; }

    [JsonPropertyName("pos")]
    public float[]? Pos { get; set; }

    [JsonPropertyName("rot")]
    public float[]? Rot { get; set; }

    [JsonPropertyName("fov")]
    public float? Fov { get; set; }

    [JsonPropertyName("mode")]
    public string? Mode { get; set; }

    [JsonPropertyName("activeCameraId")]
    public int? ActiveCameraId { get; set; }

    [JsonPropertyName("image")]
    public string? Image { get; set; }

    [JsonPropertyName("width")]
    public int? Width { get; set; }

    [JsonPropertyName("height")]
    public int? Height { get; set; }

    [JsonPropertyName("format")]
    public string? Format { get; set; }

    [JsonPropertyName("transparency")]
    public bool? Transparency { get; set; }

    [JsonPropertyName("size")]
    public int? Size { get; set; }
}

public class SceneObject
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("objectInfo")]
    public ObjectInfo ObjectInfo { get; set; } = new();

    [JsonPropertyName("children")]
    public List<SceneObject> Children { get; set; } = new();
}

public class ObjectInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("transform")]
    public Transform? Transform { get; set; }

    [JsonPropertyName("itemDetail")]
    public ItemDetail? ItemDetail { get; set; }
}

public class Transform
{
    [JsonPropertyName("pos")]
    public float[] Pos { get; set; } = new float[3];

    [JsonPropertyName("rot")]
    public float[] Rot { get; set; } = new float[3];

    [JsonPropertyName("scale")]
    public float[] Scale { get; set; } = new float[3];
}

public class ItemDetail
{
    [JsonPropertyName("group")]
    public int Group { get; set; }

    [JsonPropertyName("category")]
    public int Category { get; set; }

    [JsonPropertyName("itemId")]
    public int ItemId { get; set; }
}

public class ItemGroup
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("categoryCount")]
    public int CategoryCount { get; set; }

    [JsonPropertyName("categories")]
    public List<ItemCategory>? Categories { get; set; }
}

public class ItemCategory
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("itemCount")]
    public int ItemCount { get; set; }

    [JsonPropertyName("groupId")]
    public int GroupId { get; set; }

    [JsonPropertyName("items")]
    public List<Item>? Items { get; set; }
}

public class Item
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("properties")]
    public ItemProperties? Properties { get; set; }
}

public class ItemProperties
{
    [JsonPropertyName("isAnime")]
    public bool IsAnime { get; set; }

    [JsonPropertyName("isScale")]
    public bool IsScale { get; set; }

    [JsonPropertyName("hasColor")]
    public bool HasColor { get; set; }

    [JsonPropertyName("colorSlots")]
    public int ColorSlots { get; set; }

    [JsonPropertyName("hasPattern")]
    public bool HasPattern { get; set; }

    [JsonPropertyName("patternSlots")]
    public int PatternSlots { get; set; }

    [JsonPropertyName("isEmission")]
    public bool IsEmission { get; set; }

    [JsonPropertyName("isGlass")]
    public bool IsGlass { get; set; }

    [JsonPropertyName("bones")]
    public int Bones { get; set; }

    [JsonPropertyName("childRoot")]
    public string ChildRoot { get; set; } = string.Empty;
}

public class ItemCatalogResponse
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("command")]
    public string Command { get; set; } = string.Empty;

    [JsonPropertyName("groupId")]
    public int? GroupId { get; set; }

    [JsonPropertyName("categoryId")]
    public int? CategoryId { get; set; }

    [JsonPropertyName("data")]
    public object Data { get; set; } = new();
}

public class ScreenshotData
{
    [JsonPropertyName("image")]
    public string Image { get; set; } = string.Empty;

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("format")]
    public string Format { get; set; } = string.Empty;

    [JsonPropertyName("transparency")]
    public bool Transparency { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }
}