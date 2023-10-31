using UnityEngine;

public class MovementBoneState : StateActionBone
{
    public override void ActivateAction(Vector3 v)
    {
        Move(v);
    }

    private void Move(Vector3 direct)
    {
        foreach (BoneObject bone in ActionManager.Ford.SelectingBones)
        {
            bone.Translate(direct, SpeedShift);
        }

        ActionManager.Move(direct * SpeedShift);
    }
}