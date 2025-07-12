# KKStudioSocket Model Design Documentation

## Overview

KKStudioSocket facilitates WebSocket communication for sending and receiving request/response data with Koikatsu/KKS Studio API. This document describes the strongly-typed models designed to ensure type safety and API consistency.

## Namespace Structure

```
KKStudioSocket.Models
├── Requests/     # Request models
└── Responses/    # Response models
```

## Request Models (Requests)

### Base Class

```csharp
[Serializable]
public class BaseCommand
{
    public string type;
}
```

### Detailed Request Models

#### PingCommand
| Property | Type | Required | Description | Example |
|----------|------|----------|-------------|---------|
| `type` | string | ○ | Fixed "ping" | "ping" |
| `message` | string | - | Message | "test" |
| `timestamp` | long | - | Timestamp | 1234567890 |

#### UpdateCommand
| Property | Type | Required | Description | Target Objects |
|----------|------|----------|-------------|----------------|
| `type` | string | ○ | Fixed "update" | All |
| `command` | string | ○ | Subcommand | All |
| `id` | int | ○ | Target object ID | All |
| `pos` | float[] | - | Position [x,y,z] | All objects |
| `rot` | float[] | - | Rotation [x,y,z] (degrees) | All objects |
| `scale` | float[] | - | Scale [x,y,z] | All objects |
| `color` | float[] | - | Color [r,g,b] or [r,g,b,a] (0.0-1.0) | Items, Lights |
| `colorIndex` | int | - | Color slot index (0-3) | Items |
| `alpha` | float? | - | Transparency (0.0-1.0) | Items |
| `visible` | bool? | - | Visibility | All objects |
| `intensity` | float? | - | Light intensity (0.1-2.0) | Lights only |
| `range` | float? | - | Light range | Lights only |
| `spotAngle` | float? | - | Spot angle (1-179 degrees) | Spot lights only |
| `enable` | bool? | - | Light enabled/disabled | Lights only |

**UpdateCommand Subcommands:**
- `transform`: Transform updates (pos, rot, scale)
- `color`: Color updates (color, colorIndex, alpha)
- `visibility`: Visibility updates (visible)
- `light`: Light updates (intensity, range, spotAngle, enable)

#### AddCommand
| Property | Type | Required | Description | Target Objects |
|----------|------|----------|-------------|----------------|
| `type` | string | ○ | Fixed "add" | All |
| `command` | string | ○ | Subcommand | All |
| `group` | int | △ | Group ID | item |
| `category` | int | △ | Category ID | item |
| `itemId` | int | △ | Item ID | item |
| `lightId` | int | △ | Light type ID | light |
| `parentId` | int? | - | Parent object ID | item, light |
| `path` | string | △ | File path | character |
| `sex` | string | △ | Gender ("male"/"female") | character |
| `name` | string | △ | Name | folder, camera |

**AddCommand Subcommands:**
- `item`: Add item (group, category, itemId required)
- `light`: Add light (lightId required)
- `character`: Add character (path, sex required)
- `folder`: Add folder (name required)
- `camera`: Add camera (name required)

#### DeleteCommand
| Property | Type | Required | Description | Example |
|----------|------|----------|-------------|---------|
| `type` | string | ○ | Fixed "delete" | "delete" |
| `id` | int | ○ | Target object ID | 123 |

#### HierarchyCommand
| Property | Type | Required | Description | Target Objects |
|----------|------|----------|-------------|----------------|
| `type` | string | ○ | Fixed "hierarchy" | All |
| `command` | string | ○ | Subcommand | All |
| `childId` | int | ○ | Child object ID | All |
| `parentId` | int | △ | Parent object ID | Required for attach |

**HierarchyCommand Subcommands:**
- `attach`: Parent object (parentId required)
- `detach`: Unparent object

#### CameraCommand
| Property | Type | Required | Description | Target Objects |
|----------|------|----------|-------------|----------------|
| `type` | string | ○ | Fixed "camera" | All |
| `command` | string | ○ | Subcommand | All |
| `pos` | float[] | △ | Camera position [x,y,z] | setview |
| `rot` | float[] | △ | Camera rotation [x,y,z] | setview |
| `fov` | float | △ | Field of view | setview |
| `cameraId` | int | △ | Camera object ID | switch |

**CameraCommand Subcommands:**
- `setview`: Set camera view (pos, rot, fov required)
- `switch`: Switch to camera object (cameraId required)
- `getview`: Get current camera info
- `free`: Free camera mode

#### TreeCommand
| Property | Type | Required | Description | Example |
|----------|------|----------|-------------|---------|
| `type` | string | ○ | Fixed "tree" | "tree" |
| `depth` | int? | - | Max depth (null=unlimited) | 2 |
| `id` | int? | - | Start object ID (null=all) | 123 |

#### ItemCommand
| Property | Type | Required | Description | Target Objects |
|----------|------|----------|-------------|----------------|
| `type` | string | ○ | Fixed "item" | All |
| `command` | string | ○ | Subcommand | All |
| `groupId` | int | △ | Group ID | list-group, list-category |
| `categoryId` | int | △ | Category ID | list-category |

**ItemCommand Subcommands:**
- `list-groups`: List all groups
- `list-group`: Get group details (groupId required)
- `list-category`: Get category details (groupId, categoryId required)
- `catalog`: Get complete item catalog

#### ScreenshotCommand
| Property | Type | Required | Description | Default |
|----------|------|----------|-------------|---------|
| `type` | string | ○ | Fixed "screenshot" | "screenshot" |
| `width` | int? | - | Image width | 854 |
| `height` | int? | - | Image height | 480 |
| `transparency` | bool? | - | Include alpha channel | false |
| `mark` | bool? | - | Include capture mark | true |

## Response Models (Responses)

### Base Class Hierarchy

```
BaseResponse (abstract)
├── SuccessResponse
├── ErrorResponse
└── PongResponse
```

### Base Classes

```csharp
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
```

### Specialized Response Models

#### 1. Tree Related

```csharp
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
```

#### 2. Item Command Related

```csharp
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
public class ItemGroupDetailResponse : ItemCommandResponse
{
    public ItemGroupDetail data;
}

[Serializable]
public class ItemCategoryDetailResponse : ItemCommandResponse
{
    public ItemCategoryDetail data;
}

[Serializable]
public class ItemCatalogResponse : ItemCommandResponse
{
    public List<CatalogGroup> data;
}
```

#### 3. Screenshot Related

```csharp
[Serializable]
public class ScreenshotSuccessResponse : SuccessResponse
{
    public ScreenshotData data;
}

[Serializable]
public class ScreenshotData
{
    public string image;    // Base64 encoded image
    public int width;
    public int height;
    public string format;   // "png"
    public bool transparency;
    public int size;        // Byte count
}
```

#### 4. Camera Related

```csharp
[Serializable]
public class CameraViewResponse : SuccessResponse
{
    public float[] pos;
    public float[] rot;
    public float fov;
    public string mode;        // "free" or "object"
    public int? activeCameraId;
}
```

#### 5. Add Command Related

```csharp
[Serializable]
public class AddSuccessResponse : SuccessResponse
{
    public int objectId;
}
```

## Design Principles

### 1. Type Safety

- **No Anonymous Objects**: Complete elimination of `new { ... }` usage
- **Strong Typing**: Dedicated classes for all responses
- **Compile-time Type Checking**: Detect type inconsistencies at compile time

### 2. Consistency

- **Naming Convention**: 
  - Requests: `{CommandName}Command`
  - Responses: `{CommandName}Response` or `{CommandName}SuccessResponse`
- **Inheritance Hierarchy**: Proper inheritance from base classes
- **Serialization**: All classes have `[Serializable]` attribute

### 3. Extensibility

- **Base Classes**: Common functionality centralized in base classes
- **Namespace Separation**: Clear separation between Requests and Responses
- **Optional Support**: Flexible configuration with nullable types

## Usage Examples

### Request Sending

```csharp
var treeCmd = new TreeCommand 
{ 
    type = "tree", 
    depth = 2, 
    id = 123 
};

var json = JsonConvert.SerializeObject(treeCmd);
webSocket.Send(json);
```

### Response Receiving

```csharp
// Anonymous object (old approach)
var response = new { type = "success", data = nodes };

// Strongly-typed response (new approach)
var response = new TreeResponse { data = nodes };
var json = JsonConvert.SerializeObject(response);
```

## Error Handling

### Unified Error Response

```csharp
private void SendErrorResponse(string message)
{
    var response = new ErrorResponse(message);
    Send(JsonConvert.SerializeObject(response));
}
```

### Success Response

```csharp
private void SendSuccessResponse(string message)
{
    var response = new SuccessResponse { message = message };
    Send(JsonConvert.SerializeObject(response));
}
```

## Migration History

### Before (Dynamic Response)
```csharp
var response = new 
{
    type = "success",
    message = "Operation completed",
    data = new { id = 123, name = "Item" }
};
```

### After (Strongly-typed Response)
```csharp
var response = new ItemSuccessResponse 
{
    message = "Operation completed",
    data = new ItemData { id = 123, name = "Item" }
};
```

## Benefits

1. **Type Safety**: Compile-time type checking
2. **IntelliSense**: IDE assistance for improved development efficiency
3. **Maintainability**: Clear impact scope when types change
4. **Consistency**: Unified API response format
5. **Performance**: Serialization optimization

## Future Extensions

When adding new commands, follow these steps:

1. **Request Model**: Add class to `KKStudioSocket.Models.Requests` namespace
2. **Response Model**: Add class to `KKStudioSocket.Models.Responses` namespace
3. **CommandHandler**: Implement corresponding handler class
4. **Type Safety**: Avoid anonymous objects, use dedicated models

---

*This document describes the model design for KKStudioSocket v1.0.*