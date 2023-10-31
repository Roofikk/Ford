using Newtonsoft.Json;
using System;
using UnityEngine;

[Serializable]
public class DevBoneDataSave : BoneDataSave
{
    public string Name { get; private set; }

    [JsonConstructor]
    public DevBoneDataSave(string id, string groupId, string name, Vector3 position, Vector3 rotation) : base(id, groupId, position, rotation)
    {
        Name = name;
    }

    public DevBoneDataSave(BoneData boneData) : base(boneData)
    {
        Name = boneData.Name;
    }
}
