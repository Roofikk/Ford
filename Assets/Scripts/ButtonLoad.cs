using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonLoad : MonoBehaviour
{
    [SerializeField] private Button buttonLoad;
    [SerializeField] public string path;
    //[SerializeField] private GameObject horseAnatomy;

    [SerializeField] public LoadEvent LoadSave;
    private void OnEnable()
    {
        buttonLoad.onClick.AddListener(() => OnClicked(path));
    }

    void OnClicked(string path)
    {
        LoadSave.Invoke(path);
    }

    private void OnDisable()
    {
        buttonLoad.onClick.RemoveAllListeners();
    }
}

[System.Serializable]
public class LoadEvent : UnityEvent<string> { }
