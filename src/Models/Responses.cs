using System;
using System.Collections.Generic;

namespace KKStudioSocket.Models.Responses
{
    [Serializable]
    public abstract class BaseResponse
    {
        public required string type;
    }

    [Serializable]
    public class SuccessResponse : BaseResponse
    {
        public required string message;

        public SuccessResponse()
        {
            type = "success";
        }
    }

    [Serializable]
    public class ErrorResponse : BaseResponse
    {
        public required string message;

        public ErrorResponse()
        {
            type = "error";
        }

        public ErrorResponse(string message)
        {
            type = "error";
            this.message = message;
        }
    }

    [Serializable]
    public class PongResponse : BaseResponse
    {
        public required string message;
        public long timestamp;

        public PongResponse()
        {
            type = "pong";
        }
    }

    // Tree関連レスポンス
    [Serializable]
    public class TreeResponse : SuccessResponse
    {
        public required List<TreeNode> data;
    }

    [Serializable]
    public class TreeNode
    {
        public required string name;
        public required ObjectInfo objectInfo;
        public required List<TreeNode> children;
    }

    [Serializable]
    public class ObjectInfo
    {
        public int id;
        public required string type;
        public required Transform transform;
        public ItemDetail? itemDetail; // nullable for non-items
    }

    [Serializable]
    public class Transform
    {
        public required float[] pos; // [x, y, z]
        public required float[] rot; // [x, y, z]
        public required float[] scale; // [x, y, z]
    }

    [Serializable]
    public class ItemDetail
    {
        public int group;
        public int category;
        public int itemId;
    }

    // Item Command関連レスポンス
    [Serializable]
    public class ItemCommandResponse : SuccessResponse
    {
        public required string command;
        public int? groupId;
        public int? categoryId;
    }

    [Serializable]
    public class ItemGroupsResponse : ItemCommandResponse
    {
        public required List<ItemGroup> data;
    }

    [Serializable]
    public class ItemGroup
    {
        public int id;
        public required string name;
        public int categoryCount;
    }

    [Serializable]
    public class ItemGroupDetailResponse : ItemCommandResponse
    {
        public required ItemGroupDetail data;
    }

    [Serializable]
    public class ItemGroupDetail
    {
        public int id;
        public required string name;
        public required List<ItemCategory> categories;
    }

    [Serializable]
    public class ItemCategory
    {
        public int id;
        public required string name;
        public int itemCount;
    }

    [Serializable]
    public class ItemCategoryDetailResponse : ItemCommandResponse
    {
        public required ItemCategoryDetail data;
    }

    [Serializable]
    public class ItemCategoryDetail
    {
        public int id;
        public required string name;
        public int groupId;
        public required List<Item> items;
    }

    [Serializable]
    public class Item
    {
        public int id;
        public required string name;
        public required ItemProperties properties;
    }

    [Serializable]
    public class ItemProperties
    {
        public bool isAnime;
        public bool isScale;
        public bool hasColor;
        public int colorSlots;
        public bool hasPattern;
        public int patternSlots;
        public bool isEmission;
        public bool isGlass;
        public int bones;
        public required string childRoot;
    }

    // Add Command関連レスポンス
    [Serializable]
    public class AddSuccessResponse : SuccessResponse
    {
        public int objectId;
    }

    // Screenshot関連レスポンス
    [Serializable]
    public class ScreenshotSuccessResponse : SuccessResponse
    {
        public required ScreenshotData data;
    }

    [Serializable]
    public class ScreenshotData
    {
        public required string image;
        public int width;
        public int height;
        public required string format;
        public bool transparency;
        public int size;
    }

    // Catalog関連レスポンス
    [Serializable]
    public class ItemCatalogResponse : ItemCommandResponse
    {
        public required List<CatalogGroup> data;
    }

    [Serializable]
    public class CatalogGroup
    {
        public int id;
        public required string name;
        public required string type;
        public required List<CatalogCategory> categories;
    }

    [Serializable]
    public class CatalogCategory
    {
        public int id;
        public required string name;
        public required string type;
        public required List<CatalogItem> items;
    }

    [Serializable]
    public class CatalogItem
    {
        public int id;
        public required string name;
        public required string type;
        public int groupId;
        public int categoryId;
        public required CatalogItemProperties properties;
        public required CatalogItemFile file;
    }

    [Serializable]
    public class CatalogItemProperties
    {
        public bool isAnime;
        public bool isScale;
        public bool isEmission;
        public bool isGlass;
        public bool hasColor;
        public bool hasPattern;
        public int colorSlots;
        public int patternSlots;
        public required string childRoot;
        public int bones;
    }

    [Serializable]
    public class CatalogItemFile
    {
        public required string name;
        public required string assetBundle;
        public required string manifest;
    }

    // Camera関連レスポンス
    [Serializable]
    public class CameraViewResponse : SuccessResponse
    {
        public required float[] pos;
        public required float[] rot;
        public float fov;
        public required string mode;
        public int? activeCameraId;
    }
}