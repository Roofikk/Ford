using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneParameters
{
    private static List<object> _params = new List<object>();

    public static void AddParam<T>(T param)
    {
        _params.Add(param);
    }

    public static T GetParam<T>()
    {
        T returnParam;
        
        foreach (var param in _params)
        {
            if (param is T)
            {
                returnParam = (T)param;
                RemoveParam(returnParam);
                return (T)param;
            }
        }

        return default(T);
    }

    public static void RemoveParam<T>(T param)
    {
        if (_params.Contains(param))
        {
            _params.Remove(param);
        }
    }

    public static void ClearParams()
    {
        _params.Clear();
    }
}