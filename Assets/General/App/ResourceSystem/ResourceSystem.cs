/*
Description:
The script allows the import of resource objects.

References:
None
*/

// Libraries
using UnityEngine;

// Class
public static class ResourceSystem
{
    private const string systemPath = "system/";
    private const string menuPath = "ui/menu/";
    private const string quickmenuPath = "ui/quickmenu/";
    private const string musicPath = "audio/music/";
    private const string soundPath = "audio/sound/";
    private const string soundUIPath = "audio/ui/";
    private const string overlayPath = "ui/overlays/";
    private const string inventoryPath = "ui/inventory/";
    private const string itemDropPath = "item/item_drop/";
    private const string worldPath = "world/map/";
    private const string mapPath = "world/map/";


    // Functions
    /*
    Description:
    The function returns objects from resources by id.
    It uses the defined path to get the id.

    Arguments:
        id - Object id (name)
    
    Returns:
        object - if found
    */

    private static T LoadResource<T>(string path, string id) where T : UnityEngine.Object
    {
        try
        {
            T resource = Resources.Load<T>(path + id);

            if (resource == null)
            {
                throw new ResourceNotFoundException("Failed to load resource with ID: " + id);
            }

            return resource;
        }
        catch (ResourceNotFoundException ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error loading resource: " + e.Message);
            return null;
        }
    }

    // Custom exception class for resource not found
    private class ResourceNotFoundException : System.Exception
    {
        public ResourceNotFoundException(string message) : base(message)
        {
        }
    }

    public static GameObject GetGameObject(string id)
    {
        return LoadResource<GameObject>("", id);
    }

    // Get System Object
    public static GameObject GetSystem(string id)
    {
        return LoadResource<GameObject>(systemPath, id);
    }

    // Get Music
    public static AudioClip GetMusic(string id)
    {
        return LoadResource<AudioClip>(musicPath, id);
    }

    // Get Sound
    public static AudioClip GetSound(string id)
    {
        return LoadResource<AudioClip>(soundPath, id);
    }

    // Get UI Sound
    public static AudioClip GetSoundUI(string id)
    {
        return LoadResource<AudioClip>(soundUIPath, id);
    }

    // Get General Menu
    public static GameObject GetMenu(string id)
    {
        return LoadResource<GameObject>(menuPath, id);
    }

    // Get Quick Menu
    public static GameObject GetQuickMenu(string id)
    {
        return LoadResource<GameObject>(quickmenuPath, id);
    }

    // Get Overlay
    public static GameObject GetOverlay(string id)
    {
        return LoadResource<GameObject>(overlayPath, id);
    }

    // Item Drop
    public static GameObject GetItemDrop(string id)
    {
        return LoadResource<GameObject>(itemDropPath, id);
    }

    // Inventory
    public static GameObject GetInventory(string id)
    {
        return LoadResource<GameObject>(inventoryPath, id);
    }

    // Map
    public static GameObject GetMap(string id)
    {
        return LoadResource<GameObject>(mapPath, id);
    }

    // World
    public static GameObject GetWorld(string id)
    {
        return LoadResource<GameObject>(worldPath, id);
    }
}
