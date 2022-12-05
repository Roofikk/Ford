using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    public string Name;
    public int Age;

    [SerializeField] private List<GroupBoneObjects> _groupBones;
    [SerializeField] private bool _addGroupFromParent = true;
    [SerializeField] private ActionBoneManager _actionSystem;

    public IReadOnlyCollection<GroupBoneObjects> GroupBones => _groupBones.AsReadOnly();
    public List<Bone> SelectingBones { get; protected set; }

    private void Awake()
    {
        Initiate();
    }

    public void Initiate()
    {
        Name = "Ford";
        Age = 10;

        InitiateBone();
    }

    public void InitiateBone()
    {
        SelectingBones = new List<Bone>();

        int countChildren = transform.GetComponentsInChildren<GroupBones>().Length;
        if (_groupBones.Count != countChildren)
        {
            if (_addGroupFromParent)
            {
                Debug.LogWarning("Количество групп в списке не соответствует количеству дочерних элементов. Дочерние группы были добавлены в список из родителя");
                foreach (Transform tr in transform)
                {
                    GroupBoneObjects groupObj = null;
                    if (tr.TryGetComponent(out groupObj) && !_groupBones.Contains(groupObj))
                    {
                        _groupBones.Add(groupObj);
                    }
                }
            }
        }

        foreach (var group in _groupBones)
        {
            group.Initiate();
            foreach (var bone in group.Bones)
            {
                bone.OnClicked += UnselectAllBones;
                bone.OnChangedSelection += (selected) =>
                {
                    if (selected)
                    {
                        group.SelectingBones.Add(bone);
                        SelectingBones.Add(bone);
                    }
                    else
                    {
                        group.SelectingBones.Remove(bone);
                        SelectingBones.Remove(bone);
                    }

                    _actionSystem.SetPosition(GetCenterSelectedBones());
                    _actionSystem.CurrentState.Display(SelectingBones.Count > 0);
                };
            }
        }
    }

    public Vector3 GetCenterSelectedBones()
    {
        Vector3 sum = new Vector3();
        foreach (var bone in SelectingBones)
        {
            sum += bone.transform.position;
        }

        return sum / (SelectingBones.Count > 0 ? SelectingBones.Count : 1);
    }

    public void UnselectAllBones()
    {
        if (!(Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.LeftControl)))
            while (SelectingBones.Count > 0)
                SelectingBones[0].SetSelection(false);
    }

    public void ActivateSelection()
    {
        foreach (var group in _groupBones)
        {
            foreach (var bone in group.Bones)
            {
                (bone as BoneObject).UnlockTouch();
            }
        }
    }

    public void DeactivateSelection()
    {
        foreach (var group in _groupBones)
        {
            foreach (var bone in group.Bones)
            {
                ((BoneObject)bone).LockTouch();
            }
        }
    }
}
