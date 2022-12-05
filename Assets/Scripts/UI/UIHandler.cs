using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public static bool IsMouseOnUI { get; private set; }

    private static List<IMouseLockerUI> _mouseLockerList = new List<IMouseLockerUI>();

    private void Start()
    {
        _mouseLockerList = FindObjectsOfType<MonoBehaviour>().OfType<IMouseLockerUI>().ToList();

        if (_mouseLockerList.Count == 0)
        {
            Debug.LogWarning("Не найдены объекты, которые надо заблокировать для мыши");
            return;
        }
    }

    void Update()
    {
        IsMouseOnUI = IsMouseOverUI() ? true : false;
    }

    public static void LockMouseUI()
    {
        foreach (var obj in _mouseLockerList)
        {
            obj.Lock();
        }
    }

    public static void UnlockMouseUI()
    {
        foreach (var obj in _mouseLockerList)
        {
            obj.Unlock();
        }
    }

    private bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
