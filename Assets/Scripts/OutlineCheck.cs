using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineCheck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitArrow;
        Ray rayCheck = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerMask = 1 << 4;

        if (Physics.Raycast(rayCheck, out hitArrow, 1000, layerMask) && !Input.GetKey(KeyCode.LeftShift) && !UiHandlerPlayMode.cursorOnUi)
        {
            if(hitArrow.transform == transform)
            {
                GetComponent<Outline>().enabled = true;
            }
            else
            {
                GetComponent<Outline>().enabled = false;
            }
        }
        else
        {
            GetComponent<Outline>().enabled = false;
        }
    }
}
