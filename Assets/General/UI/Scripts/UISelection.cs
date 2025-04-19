using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

// -----------------------------------------------------------------------------
// Class to handle UI Selection
public class UISelection : MonoBehaviour
{
    public static UISelection Instance { get; private set; }

    [Header("Components")]
    public EventSystem eventSystem;
    public bool mouseLock;

    [Header("Selection")]
    public bool isSelected;
    public GameObject selected;
    public GameObject shouldSelected;

    // -----------------------------------------------------------------------------
    // Singleton Instance
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

    // -----------------------------------------------------------------------------
    // Update
    private void Update()
    {
        HandleUISelection();
        HandleMouseLock();
    }

    // -----------------------------------------------------------------------------
    // checks if mouse should be locked or not
    private void HandleMouseLock()
    {
        if (UIManager.Instance)
        {
            mouseLock = !UIManager.Instance.uiOpen;
        }

        if (!mouseLock)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // -----------------------------------------------------------------------------
    // UI Selection
    private void HandleUISelection()
    {
        // check for current selection
        selected = eventSystem.currentSelectedGameObject;

        // set State
        if (selected != null)
        {
            isSelected = true;
        }
        else
        {
            isSelected = false;
        }
    }

    // -----------------------------------------------------------------------------
    // Sets main selection
    public void Set(GameObject selection)
    {
        eventSystem.SetSelectedGameObject(null);
        shouldSelected = selection;
        eventSystem.SetSelectedGameObject(selection);
    }

    // sets current selection
    public void Selected(GameObject selection)
    {
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(selection);
    }

    // selects should selection
    public void ShouldSelected()
    {
        Selected(shouldSelected);
    }
}
