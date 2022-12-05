using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ClickManager
{
    private static int _clicks;
    private static float _clickDelay;
    private static float _clickTime;

    public static void CheckDoubleClicks()
    {
        _clicks++;
        if (_clicks == 1)
            _clickTime = Time.time;


    }
}
