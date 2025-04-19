using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HandleSelectable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler, ISelectHandler, IDeselectHandler
{
    [Header("State")]
    public bool highlighted;
    public bool selected;
    public bool pressed;

    [Header("Audio")]
    public bool clickSound;

    public void Update()
    {
        selected = this.gameObject == UISelection.Instance.eventSystem.currentSelectedGameObject;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
        if (clickSound)
        {
            AudioManager.Instance?.LoadPlayUI("click");
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
    }

    public void OnSelect(BaseEventData eventData)
    {
        //selected = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        //selected = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //highlighted = true;
        UISelection.Instance.Selected(this.gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //highlighted = false;
        UISelection.Instance.Selected(null);
    }
}
