using System.Collections.Generic;
using UnityEngine;

public class RotationBoneState : StateActionBone
{
    public override void Init(ActionBoneManager stateMachine)
    {
        base.Init(stateMachine);
    }
    public override void ActivateAction(Vector3 v, float power)
    {
        Rotate(v, power);
    }

    private void Rotate(Vector3 eulerAngle, float angle)
    {
        foreach (BoneObject bone in ActionManager.Ford.SelectingBones)
        {
            bone.Rotate(eulerAngle, angle);
        }
    }
}
