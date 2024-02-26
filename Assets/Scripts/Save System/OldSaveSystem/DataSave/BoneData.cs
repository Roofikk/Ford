using UnityEngine;

namespace Ford.SaveSystem.Ver1.Data
{
    public class BoneData
    {
        public string Id;
        public string GroupId;
        public string Name;
        public Vector3 Position;
        public Vector3 Rotation;

        public BoneData(string id, string groupId, string name, Vector3 position, Vector3 rotation)
        {
            Id = id;
            GroupId = groupId;
            Name = name;
            Position = position;
            Rotation = rotation;
        }
    }
}
