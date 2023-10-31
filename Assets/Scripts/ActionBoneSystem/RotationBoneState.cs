using UnityEngine;

public class RotationBoneState : StateActionBone
{
    public override void ActivateAction(Vector3 v)
    {
        Rotate(v);
    }

    private void Rotate(Vector3 eulerAngle)
    {
        foreach (BoneObject bone in ActionManager.Ford.SelectingBones)
        {
            bone.Rotate(eulerAngle, SpeedShift);
        }
    }
}
