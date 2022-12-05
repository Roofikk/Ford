using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Bone", menuName = "Bone/Create new Bone")]
public class BoneData : ScriptableObject
{
    public string Id;
    public string Name;
    public Vector3 Position;
    public Vector3 Rotation;

    public BoneData(string id, string name, Vector3 position, Vector3 rotation)
    {
        Id = id;
        Name = name;
        Position = position;
        Rotation = rotation;
    }
}
