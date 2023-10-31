using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    [SerializeField] private List<GroupBoneObjects> _groupBones;
    [SerializeField] private bool _addGroupFromParent = true;
    [SerializeField] private ActionBoneManager _actionSystem;

    public IReadOnlyCollection<GroupBoneObjects> GroupBones => _groupBones.AsReadOnly();
    public List<Bone> SelectingBones { get; protected set; }
    public HorseData Data { get; private set; }

    private enum State
    {
        NormalMode,
        DeveloperMode
    }

    private void Awake()
    {
        Initiate();
    }

    public void Initiate()
    {
        if ((Data = SceneParameters.GetParam<DevHorseData>()) != null)
        {
            GameManager.Instance.SetDeveloperMode();
            InitiateBone();

            DevHorseSaveData dataSave = SceneParameters.GetParam<DevHorseSaveData>();

            if (dataSave != null)
            {
                SetDataBoneFromSave(dataSave);
            }
            return;
        }

        InitiateBone();

        Data = SceneParameters.GetParam<HorseData>();

        if (Data == null)
            Debug.LogError("Информация о лошади отсутствует. HorseData is null");

        HorseSaveData Save = SceneParameters.GetParam<HorseSaveData>();

        if (Save != null)
        {
            SetDataBoneFromSave(Save);
        }
    }

    private void InitiateBone()
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

    private void SetDataBoneFromSave(HorseSaveData save)
    {
        if (save == null && save as DevHorseSaveData == null)
            return;

        foreach(var group in _groupBones)
        {
            foreach(var bone in group.Bones.Cast<BoneObject>())
            {
                if (save as DevHorseSaveData != null)
                {
                    var findingBone = (save as DevHorseSaveData).DevBones.Find((b) => b.Id == bone.BoneData.Id);

                    if (findingBone != null)
                    {
                        bone.SetPosition(findingBone.Position);
                        bone.SetRotation(findingBone.Rotation);
                        bone.EditBoneName(findingBone.Name);
                    }
                    else
                    {
                        bone.SetPosition(bone.BoneData.Position);
                        bone.SetRotation(bone.BoneData.Rotation);
                    }
                }
                else
                {
                    if (save.Bones == null)
                        return;

                    var findingBone = save.Bones.Find((b) => b.Id == bone.BoneData.Id);

                    if (findingBone != null)
                    {
                        bone.SetPosition(findingBone.Position);
                        bone.SetRotation(findingBone.Rotation);
                    }
                    else
                    {
                        bone.SetPosition(bone.BoneData.Position);
                        bone.SetRotation(bone.BoneData.Rotation);
                    }
                }
            }
        }
    }

    public Vector3 GetCenterSelectedBones()
    {
        Vector3 sum = new();
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

    public List<BoneDataSave> GetBonesForSave()
    {
        List<BoneDataSave> bones = new();

        foreach (var group in _groupBones)
        {
            foreach (BoneObject bone in group.Bones.Cast<BoneObject>())
            {
                if (bone.ShiftPosition != Vector3.zero || (bone.ShiftRotation - bone.DefaultRotation).magnitude > 0.1)
                {
                    bones.Add(new(bone.BoneData.Id, bone.BoneData.GroupId, bone.transform.position, bone.transform.rotation.eulerAngles));
                }
            }
        }

        return bones;
    }

    public List<BoneDataSave> GetAllBonesForSave()
    {
        List<BoneDataSave> bones = new();

        foreach (var group in _groupBones)
        {
            foreach (BoneObject bone in group.Bones.Cast<BoneObject>())
            {
                bones.Add(new(bone.BoneData));
            }
        }

        return bones;
    }

    public List<DevBoneDataSave> GetDevBonesData()
    {
        List<DevBoneDataSave> bones = new();

        foreach (var group in _groupBones)
        {
            foreach (BoneObject bone in group.Bones.Cast<BoneObject>())
            {
                bones.Add(new(bone.BoneData));
            }
        }

        return bones;
    }
}