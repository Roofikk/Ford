using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MovementBoneState : StateActionBone
{
    public override void LateTic()
    {
        base.LateTic();
    }

    public override void ActivateAction(Vector3 v, float power)
    {
        Move(v, power);
    }

    private void Move(Vector3 direct, float power)
    {
        foreach (BoneObject bone in ActionManager.Ford.SelectingBones)
        {
            bone.Translate(direct, power);
        }

        ActionManager.Move(direct * power);
    }
}