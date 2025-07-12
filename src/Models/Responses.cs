using System;
using System.Collections.Generic;

namespace KKStudioSocket.Models.Responses
{
    [Serializable]
    public abstract class BaseResponse
    {
        public string type;
    }

    [Serializable]
    public class SuccessResponse : BaseResponse
    {
        public string message;
        
        public SuccessResponse()
        {
            type = "success";
        }
    }

    [Serializable]
    public class ErrorResponse : BaseResponse
    {
        public string message;
        
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
        public string message;
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
        public List<TreeNode> data;
    }

    [Serializable]
    public class TreeNode
    {
        public string name;
        public ObjectInfo objectInfo;
        public List<TreeNode> children;
    }

    [Serializable]
    public class ObjectInfo
    {
        public int id;
        public string type;
        public Transform transform;
        public ItemDetail itemDetail; // nullable for non-items
    }

    [Serializable]
    public class Transform
    {
        public float[] pos; // [x, y, z]
        public float[] rot; // [x, y, z]
        public float[] scale; // [x, y, z]
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
        public string command;
        public int? groupId;
        public int? categoryId;
    }

    [Serializable]
    public class ItemGroupsResponse : ItemCommandResponse
    {
        public List<ItemGroup> data;
    }

    [Serializable]
    public class ItemGroup
    {
        public int id;
        public string name;
        public int categoryCount;
    }

    [Serializable]
    public class ItemGroupDetailResponse : ItemCommandResponse
    {
        public ItemGroupDetail data;
    }

    [Serializable]
    public class ItemGroupDetail
    {
        public int id;
        public string name;
        public List<ItemCategory> categories;
    }

    [Serializable]
    public class ItemCategory
    {
        public int id;
        public string name;
        public int itemCount;
    }

    [Serializable]
    public class ItemCategoryDetailResponse : ItemCommandResponse
    {
        public ItemCategoryDetail data;
    }

    [Serializable]
    public class ItemCategoryDetail
    {
        public int id;
        public string name;
        public int groupId;
        public List<Item> items;
    }

    [Serializable]
    public class Item
    {
        public int id;
        public string name;
        public ItemProperties properties;
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
        public string childRoot;
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
        public ScreenshotData data;
    }

    [Serializable]
    public class ScreenshotData
    {
        public string image;
        public int width;
        public int height;
        public string format;
        public bool transparency;
        public int size;
    }

    // Catalog関連レスポンス
    [Serializable]
    public class ItemCatalogResponse : ItemCommandResponse
    {
        public List<CatalogGroup> data;
    }

    [Serializable]
    public class CatalogGroup
    {
        public int id;
        public string name;
        public string type;
        public List<CatalogCategory> categories;
    }

    [Serializable]
    public class CatalogCategory
    {
        public int id;
        public string name;
        public string type;
        public List<CatalogItem> items;
    }

    [Serializable]
    public class CatalogItem
    {
        public int id;
        public string name;
        public string type;
        public int groupId;
        public int categoryId;
        public CatalogItemProperties properties;
        public CatalogItemFile file;
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
        public string childRoot;
        public int bones;
    }

    [Serializable]
    public class CatalogItemFile
    {
        public string name;
        public string assetBundle;
        public string manifest;
    }

    // Camera関連レスポンス
    [Serializable]
    public class CameraViewResponse : SuccessResponse
    {
        public float[] pos;
        public float[] rot;
        public float fov;
        public string mode;
        public int? activeCameraId;
    }
}