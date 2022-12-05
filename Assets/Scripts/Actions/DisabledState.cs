using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisabledState : ActionStateBase
{
    public override void Enter(ActionBone action)
    {
        action.Disable();
        action.SetColor(action.Colors.Deactivated);
        action.Outline.enabled = false;
    }

    public override void Exit(ActionBone action)
    {
        action.Enable();
        action.Outline.enabled = true;
    }
}
