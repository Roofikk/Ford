using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outline_Ray : MonoBehaviour {

    public bool IsActive;
    public GameObject Object_Hit;

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        var mainCamera = FindCamera();
        RaycastHit hit = new RaycastHit();
        if (
            !Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition).origin,
                             mainCamera.ScreenPointToRay(Input.mousePosition).direction, out hit, 100,
                             Physics.DefaultRaycastLayers))

        {
            return;
        }
        if (hit.collider.tag == "GravityObject")
        {
            Object_Hit = hit.collider.gameObject;
            IsActive = true;
        }
        else
        {
            Object_Hit = null;
            IsActive = false;
        }
    }
    private Camera FindCamera()
    {
        if (GetComponent<Camera>())
        {
            return GetComponent<Camera>();
        }

        return Camera.main;
    }
}
