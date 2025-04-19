using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

public class MenuManager : MonoBehaviour
{
    [System.Serializable]
    private class MenuData
    {
        public string stateName;
        public Selectable selectable;
        public CanvasGroup menuPage;
    }

    [Header("General")]
    public string menuID;
    public bool isActiveMenu = false;
    public int priority = 0;
    public string currentStateName;

    [Header("MenuPages")]
    [SerializeField] private List<MenuData> menuDataList;

    [Header("MenuCanvas")]
    public CanvasGroup menuGroup;

    [Header("Events")]
    public UnityEvent closeEvent;

    public string GetState()
    {
        return currentStateName;
    }

    public void StartMenu()
    {

    }

    public void CloseMenu()
    {
        if (closeEvent != null)
        {
            closeEvent.Invoke();
        }
    }

    public void DestroyMenu()
    {
        if (this.gameObject != null)
        {
            UIManager.Instance.RemoveMenu(this.gameObject);
        }
    }

    public void SetMenuActive(bool newState)
    {
        isActiveMenu = newState;
        menuGroup.interactable = newState;
        menuGroup.blocksRaycasts = newState;
        // enuGroup.alpha = newState ? 1f : 0f;
        if (newState)
        {
            SetMenuState(currentStateName);
        }
    }

    public void SetMenuState(string newState)
    {
        foreach (var menuData in menuDataList)
        {
            bool isActive = menuData.stateName == newState;
            menuData.menuPage.interactable = isActive;
            menuData.menuPage.alpha = isActive ? 1f : 0f;
            menuData.menuPage.blocksRaycasts = isActive;

            if (isActive)
            {
                currentStateName = menuData.stateName;
                UISelection.Instance.Set(menuData.selectable.gameObject);
            }
        }
    }

    public bool ActiveMenuBool(string _id)
    {
        bool isActive = false;

        if (isActiveMenu)
        {
            isActive = menuID == _id;
        }

        return isActive;
    }
}
