using Newtonsoft.Json;
using System;
using System.Collections.Generic;

[Serializable]
public class DevHorseSaveData : HorseSaveData
{
    public List<DevBoneDataSave> DevBones {  get; set; }

    [JsonConstructor]
    public DevHorseSaveData(string name, string description, DateTime date, List<BoneDataSave> bones, string horseId, List<DevBoneDataSave> devBones) 
        : base(name, description, date, bones, horseId)
    {
        DevBones = devBones;
    }
}
