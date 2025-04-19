using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

// Class containing List info for menus
[System.Serializable]
public class MenuListObject
{
    public GameObject menuObject;
    public MenuManager menuManager;
}

// [System.Serializable]
// public class OverlayItem : MonoBehaviour
// {
//     [SerializeField] private string id = "";
//     [SerializeField] private GameObject overlayObject;

//     public void SetVisible(bool isVisible)
//     {
//         id = "";
//         gameObject.SetActive(isVisible);
//     }
// }

// -----------------------------------------------------------------------------
// Class to handle User Interfaces
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Menu")]
    public bool uiOpen;
    public List<MenuListObject> uiMenus = new List<MenuListObject>();

    // [Header("Overlay")]
    // [SerializeField] private GameObject overlayCanvas;
    // public List<OverlayItem> overlayItems = new List<OverlayItem>(); // List of all overlay items

    //------------------------------------------------------------------------------------ Start & Update

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    //------------------------------------------------------------------------------------ Menus

    // Checks if any UI is open
    public bool isUIOpen()
    {
        return uiOpen;
    }

    // Loads UI into scene and sets the first menu as active
    public void LoadMenu(string id)
    {
        GameObject newMenu = ResourceSystem.GetMenu(id);
        if (newMenu != null)
        {
            var m = Instantiate(newMenu, transform.position, transform.rotation, this.gameObject.transform);
            var mManager = m.GetComponent<MenuManager>(); // Get the MenuManager component from the newMenu GameObject
            var menuListObject = new MenuListObject
            {
                menuObject = m,
                menuManager = mManager
            };
            uiMenus.Add(menuListObject); // Add the instantiated menu and its MenuManager to the list
            InputManager.Instance.ResetUIInput();
            CheckMenus();
        }
        else
        {
            Debug.Log(id + " not found");
        }
    }

    public void RemoveMenu(GameObject menu)
    {
        var menuListObject = uiMenus.Find(mlo => mlo.menuObject == menu);
        if (menuListObject != null)
        {
            uiMenus.Remove(menuListObject); // Remove the menu from the list

            // Destroy the menu GameObject
            Destroy(menu);

            CheckMenus(); // Check the remaining menus and set the first one as active if needed
        }
        else
        {
            Debug.Log("Menu not found in the list.");
        }
    }

    public void CheckMenus()
    {
        if (uiMenus.Count > 0)
        {
            int highestPriorityIndex = 0;
            int highestPriority = uiMenus[0].menuManager.priority;

            // Find the menu with the highest priority
            for (int i = 1; i < uiMenus.Count; i++)
            {
                if (uiMenus[i].menuManager.priority > highestPriority)
                {
                    highestPriority = uiMenus[i].menuManager.priority;
                    highestPriorityIndex = i;
                }
            }

            // Activate the menu with the highest priority
            for (int i = 0; i < uiMenus.Count; i++)
            {
                bool isInteractable = i == highestPriorityIndex;
                uiMenus[i].menuManager.SetMenuActive(isInteractable);
            }

            uiOpen = true;
        }
        else
        {
            uiOpen = false;
        }
    }

    // Clears UI from scene, but keeps the console open
    public void ClearAllMenus()
    {
        List<MenuListObject> menusToClose = new List<MenuListObject>();

        foreach (MenuListObject menuListObject in uiMenus)
        {
            // Check if the current menu is not the console
            if (menuListObject.menuManager.menuID != "console")
            {
                // Add the menu to the list of menus to close
                menusToClose.Add(menuListObject);
            }
        }

        // Close the menus outside the foreach loop
        foreach (MenuListObject menuToClose in menusToClose)
        {
            menuToClose.menuManager.CloseMenu();
        }

        CheckMenus();
    }

    // // Adds an overlay item to the list
    // public void AddOverlayItem(OverlayItem overlayItem)
    // {

    // }

    // // Loads UI overlay into scene
    // public void LoadOverlay(string id)
    // {

    // }

    // // Clears UI overlay from scene
    // public void ClearOverlay()
    // {

    // }
}
