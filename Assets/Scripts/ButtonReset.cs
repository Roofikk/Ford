using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ButtonReset : MonoBehaviour
{
    public void SetDefaultPosition()
    {
        BoneObject[] bones = GetComponentsInChildren<BoneObject>();
        foreach (var e in bones)
        {
            e.transform.position = e.DefaultPosition;
            e.transform.localEulerAngles = e.DefaultRotation;
        }
    }
}
