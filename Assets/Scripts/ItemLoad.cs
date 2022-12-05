using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLoad : MonoBehaviour
{
    public string path;
    public bool isSelected;
    public static ItemLoad prev;

    public new string name { get; set; }
}
