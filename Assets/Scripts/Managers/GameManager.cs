﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Settings _settings;

    private bool _devMode = false;
    private List<IMouseTouchLocker> _touchLockerList = new();

    public static GameManager Instance { get; private set; }
    public Settings Settings { get { return _settings; } }
    public bool DevMode { get { return _devMode; } }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        AddLockerObjects();

        UIHandler.OnMouseEnterUI += LockTouchObjects;
        UIHandler.OnMouseLeaveUI += UnlockTouchObjects;

        _settings.Initiate();
    }

    private void OnDestroy()
    {
        UIHandler.OnMouseEnterUI -= LockTouchObjects;
        UIHandler.OnMouseLeaveUI -= UnlockTouchObjects;

        _settings.Save();
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

    public void ExitProject()
    {
        SceneManager.LoadSceneAsync(0);
    }

    internal void SetDeveloperMode()
    {
        _devMode = true;
        UiManager.Instance.ShowDevAlertPage();
    }
}