using Newtonsoft.Json;
using System;
using UnityEngine;

[Serializable]
public class BoneDataSave
{
    public string Id { get; private set; }
    public string GroupId { get; private set; }
    public Vector3 Position { get; private set; }
    public Vector3 Rotation { get; set; }

    [JsonConstructor]
    public BoneDataSave(string id, string groupId, Vector3 position, Vector3 rotation)
    {
        Id = id;
        GroupId = groupId;
        Position = position;
        Rotation = rotation;
    }

    public BoneDataSave(BoneData boneData)
    {
        Id = boneData.Id;
        GroupId = boneData.GroupId;
        Position = boneData.Position;
        Rotation = boneData.Rotation;
    }
}
