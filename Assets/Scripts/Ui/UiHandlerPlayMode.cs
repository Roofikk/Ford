using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UiHandlerPlayMode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public static bool cursorOnUi;

    private void OnGUI()
    {
        if (Input.GetMouseButton(2))
        {
            //GUI.Box(new Rect())
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cursorOnUi = true;
        Disable();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cursorOnUi = false;
        Enable();
    }

    public void Enable()
    {

    }

    public void Disable()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }
}