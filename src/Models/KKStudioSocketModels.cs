using System.Text.Json.Serialization;

namespace KoikatuMCP.Models;

public class BaseCommand
{
    [JsonPropertyName("type")]
    public string type { get; set; } = string.Empty;
}

public class PingCommand : BaseCommand
{
    [JsonPropertyName("message")]
    public string message { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public long timestamp { get; set; }
}

public class UpdateCommand : BaseCommand
{
    [JsonPropertyName("command")]
    public string command { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public int id { get; set; }

    [JsonPropertyName("pos")]
    public float[]? pos { get; set; }

    [JsonPropertyName("rot")]
    public float[]? rot { get; set; }

    [JsonPropertyName("scale")]
    public float[]? scale { get; set; }

    [JsonPropertyName("color")]
    public float[]? color { get; set; }

    [JsonPropertyName("colorIndex")]
    public int colorIndex { get; set; }

    [JsonPropertyName("alpha")]
    public float? alpha { get; set; }

    [JsonPropertyName("visible")]
    public bool? visible { get; set; }

    [JsonPropertyName("intensity")]
    public float? intensity { get; set; } // Light intensity (0.1-2.0)

    [JsonPropertyName("range")]
    public float? range { get; set; } // Light range (Point: 0.1-100, Spot: 0.5-100)

    [JsonPropertyName("spotAngle")]
    public float? spotAngle { get; set; } // Spot angle (1-179 degrees)

    [JsonPropertyName("enable")]
    public bool? enable { get; set; } // Light enabled/disabled
}

public class AddCommand : BaseCommand
{
    [JsonPropertyName("command")]
    public string command { get; set; } = string.Empty;

    [JsonPropertyName("group")]
    public int group { get; set; }

    [JsonPropertyName("category")]
    public int category { get; set; }

    [JsonPropertyName("itemId")]
    public int itemId { get; set; }

    [JsonPropertyName("lightId")]
    public int lightId { get; set; }

    [JsonPropertyName("parentId")]
    public int? parentId { get; set; }

    [JsonPropertyName("path")]
    public string path { get; set; } = string.Empty;

    [JsonPropertyName("sex")]
    public string sex { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string name { get; set; } = string.Empty;
}

public class HierarchyCommand : BaseCommand
{
    [JsonPropertyName("command")]
    public string command { get; set; } = string.Empty;

    [JsonPropertyName("childId")]
    public int childId { get; set; }

    [JsonPropertyName("parentId")]
    public int parentId { get; set; } // Required for attach, ignored for detach
}

public class DeleteCommand : BaseCommand
{
    [JsonPropertyName("id")]
    public int id { get; set; }
}

public class CameraCommand : BaseCommand
{
    [JsonPropertyName("command")]
    public string command { get; set; } = string.Empty;

    [JsonPropertyName("pos")]
    public float[]? pos { get; set; }

    [JsonPropertyName("rot")]
    public float[]? rot { get; set; }

    [JsonPropertyName("fov")]
    public float fov { get; set; }

    [JsonPropertyName("cameraId")]
    public int cameraId { get; set; } // For switching to specific camera object
}

public class ItemCommand : BaseCommand
{
    [JsonPropertyName("command")]
    public string command { get; set; } = string.Empty;

    [JsonPropertyName("groupId")]
    public int groupId { get; set; } = -1;

    [JsonPropertyName("categoryId")]
    public int categoryId { get; set; } = -1;
}

public class TreeCommand : BaseCommand
{
    [JsonPropertyName("depth")]
    public int? depth { get; set; } // Maximum depth to retrieve (null = unlimited)

    [JsonPropertyName("id")]
    public int? id { get; set; } // Specific object ID to start from (null = all roots)
}

public class ScreenshotCommand : BaseCommand
{
    [JsonPropertyName("width")]
    public int? width { get; set; } // Screenshot width (default: 854)

    [JsonPropertyName("height")]
    public int? height { get; set; } // Screenshot height (default: 480)

    [JsonPropertyName("transparency")]
    public bool? transparency { get; set; } // Include alpha channel (default: false)

    [JsonPropertyName("mark")]
    public bool? mark { get; set; } // Include capture mark (default: true)
}