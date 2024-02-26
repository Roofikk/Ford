using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ford.SaveSystem.Ver1.Data
{
    public class HorseSaveData
    {
        public string Id { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public List<BoneDataSave> Bones { get; set; }

        public string HorseId { get; set; }

        [JsonIgnore] public string PathSave => Id + ".json";

        public HorseSaveData(string name, string description, DateTime date, List<BoneDataSave> bones, string horseId)
        {
            Name = name;
            Description = description;
            Date = date;
            Bones = bones;
            HorseId = horseId;

            var hash = new Hash128();
            hash.Append(Name);
            hash.Append(Date.ToString());
            hash.Append(HorseId);
            hash.Append(DateTime.Now.ToString());
            Id = hash.ToString();
        }

        [JsonConstructor]
        public HorseSaveData(string id, string name, string description, DateTime date, List<BoneDataSave> bones, string horseId)
        {
            Id = id;
            Name = name;
            Description = description;
            Date = date;
            Bones = bones;
            HorseId = horseId;
        }
    }
}
