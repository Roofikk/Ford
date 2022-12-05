using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private List<IMouseTouchLocker> _touchLockerList = new List<IMouseTouchLocker>();

    private void Awake()
    {
        if (Instance == null) 
            Instance = this;
    }

    private void Start()
    {
        AddLockerObjects();
    }

    private void AddLockerObjects()
    {
        _touchLockerList = FindObjectsOfType<MonoBehaviour>(true).OfType<IMouseTouchLocker>().ToList();
    }

    public void LockTouchObjects()
    {
        foreach (var obj in _touchLockerList)
        {
            obj.LockTouch();
        }
    }

    public void UnlockTouchObjects()
    {
        foreach (var obj in _touchLockerList)
        {
            obj.UnlockTouch();
        }
    }

    public void CheckLoadScene()
    {
        SceneManager.LoadSceneAsync(1);
    }
}