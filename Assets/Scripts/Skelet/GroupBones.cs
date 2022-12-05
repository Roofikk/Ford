using System.Collections.Generic;
using UnityEngine;

public abstract class GroupBones : MonoBehaviour
{
    [SerializeField] protected List<Bone> _bones;
    public IReadOnlyCollection<Bone> Bones => _bones.AsReadOnly();

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
