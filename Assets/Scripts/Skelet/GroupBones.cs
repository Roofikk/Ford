using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class GroupBones : MonoBehaviour
{
    [SerializeField] private string _id;
    [SerializeField] protected List<Bone> _bones;

    public string Id { get { return _id; } }

    public List<Bone> Bones { get { return _bones; } }
    public List<Bone> SelectingBones { get; protected set; }

    public string Name = "Default";

    public virtual void Initiate()
    {
        SelectingBones = new List<Bone>();
        if (Name == "Default" || string.IsNullOrEmpty(Name))
        {
            Name = name;
        }
    }
}
