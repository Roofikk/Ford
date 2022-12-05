using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Context : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject _context;
    private static bool _isEntering;
    public static bool IsEntering { get { return _isEntering; } }
    private static bool _isShowed;
    public static bool IsShowed { get { return _isShowed; } }

    private void Start()
    {
        _context = gameObject;
        _context.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isEntering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isEntering = false;
    }

    private void OnDisable()
    {
        _isEntering = false;
        _isShowed = false;
    }

    private void OnEnable()
    {
        _isShowed = true;
    }
}
