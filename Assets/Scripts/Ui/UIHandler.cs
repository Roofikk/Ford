using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UIHandler : MonoBehaviour
{
    private static List<IMouseLockerUI> _mouseLockerList = new();
    private bool _isEnterMouseUi = false;
    private bool _isLeaveMouseUi = true;
    public static bool IsMouseOnUI { get; private set; }

    public static event Action OnMouseEnterUI;
    public static event Action OnMouseLeaveUI;

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
        IsMouseOnUI = IsMouseOverUI();
        ActivateEvents();
    }

    private void ActivateEvents()
    {
        if (IsMouseOnUI && !_isEnterMouseUi)
        {
            _isEnterMouseUi = true;
            _isLeaveMouseUi = false;
            OnMouseEnterUI?.Invoke();
        }

        if (!IsMouseOnUI && !_isLeaveMouseUi)
        {
            _isEnterMouseUi = false;
            _isLeaveMouseUi = true;
            OnMouseLeaveUI?.Invoke();
        }
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
