using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class GroupBonesUI : GroupBones
{
    [SerializeField] private Transform _contentBonesUI;
    [SerializeField] private TextMeshProUGUI _headerName;
    [SerializeField] private Button _button;

    public Transform ContentBonesUI { get { return _contentBonesUI; } }

    public void Initiate(GroupBones group)
    { 
        Initiate();
        Name = group.Name;
        _headerName.text = Name;
        _bones = new List<Bone>();

        foreach (var bone in group.Bones)
        {
            BoneUI boneUi = UiManager.Instance.CreateBoneUI(_contentBonesUI);
            boneUi.Initiate((BoneObject)bone);

            _bones.Add(boneUi);
        }
    }
}