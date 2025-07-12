using System;

namespace KKStudioSocket.Models.Requests
{
    [Serializable]
    public class BaseCommand
    {
        public string type;
    }

    [Serializable]
    public class PingCommand : BaseCommand
    {
        public string message;
        public long timestamp;
    }

    [Serializable]
    public class UpdateCommand : BaseCommand
    {
        public string command;
        public int id;
        public float[] pos;
        public float[] rot;
        public float[] scale;
        public float[] color;
        public int colorIndex;
        public float? alpha;
        public bool? visible;
        public float? intensity; // Light intensity (0.1-2.0)
        public float? range; // Light range (Point: 0.1-100, Spot: 0.5-100)
        public float? spotAngle; // Spot angle (1-179 degrees)
        public bool? enable; // Light enabled/disabled
    }

    [Serializable]
    public class AddCommand : BaseCommand
    {
        public string command;
        public int group;
        public int category;
        public int itemId;
        public int lightId;
        public int? parentId;
        public string path;
        public string sex;
        public string name;
    }

    [Serializable]
    public class HierarchyCommand : BaseCommand
    {
        public string command;
        public int childId;
        public int parentId; // Required for attach, ignored for detach
    }

    [Serializable]
    public class DeleteCommand : BaseCommand
    {
        public int id;
    }

    [Serializable]
    public class CameraCommand : BaseCommand
    {
        public string command;
        public float[] pos;
        public float[] rot;
        public float fov;
        public int cameraId; // For switching to specific camera object
    }

    [Serializable]
    public class ItemCommand : BaseCommand
    {
        public string command;
        public int groupId = -1;
        public int categoryId = -1;
    }

    [Serializable]
    public class TreeCommand : BaseCommand
    {
        public int? depth; // Maximum depth to retrieve (null = unlimited)
        public int? id; // Specific object ID to start from (null = all roots)
    }

    [Serializable]
    public class ScreenshotCommand : BaseCommand
    {
        public int? width; // Screenshot width (default: 854)
        public int? height; // Screenshot height (default: 480)
        public bool? transparency; // Include alpha channel (default: false)
        public bool? mark; // Include capture mark (default: true)
    }
}