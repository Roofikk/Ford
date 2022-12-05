using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionStateBase
{
    public abstract void Enter(ActionBone action);

    public abstract void Exit(ActionBone action);
}
