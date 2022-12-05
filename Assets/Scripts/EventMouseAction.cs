using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventMouseAction : MonoBehaviour
{
    private static bool cursorOnUi;
    GraphicRaycaster raycaster;

    public static bool CursourOnUi { get { return cursorOnUi; } }

    private void Update()
    {
        //Set up the new Pointer Event
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        List<RaycastResult> results = new List<RaycastResult>();

        //Raycast using the Graphics Raycaster and mouse click position
        pointerData.position = Input.mousePosition;
        this.raycaster.Raycast(pointerData, results);

        cursorOnUi = results.Count != 0 ? true : false;

        //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
        //foreach (RaycastResult result in results)
        //{
        //    Debug.Log("Hit " + result.gameObject.name);
        //}
    }
    private void Awake()
    {
        raycaster = GetComponent<GraphicRaycaster>();
    }
}
