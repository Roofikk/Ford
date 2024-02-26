using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ford.SaveSystem.Ver1.Data
{
    public class HorseData
    {
        public string Id { get; protected set; }
        public string Name { get; set; }
        public string Birthday { get; set; }
        public string Sex { get; set; }
        public string Description { get; set; }

        public string OwnerName { get; set; }
        public string Locality { get; set; }
        public string PhoneNumber { get; set; }

        public DateTime DateCreation { get; set; }
        public List<string> SavesId { get; set; }

        [JsonIgnore] public int Age { get; private set; }
        [JsonIgnore] public string PathSave => Id + ".json";

        public HorseData(string name, string sex, string birthday, string description, string ownerName, string locality, string phoneNumber, List<string> savesId)
        {
            Name = name;
            Sex = sex;
            Birthday = birthday;
            Description = description;

            OwnerName = ownerName;
            Locality = locality;
            PhoneNumber = phoneNumber;

            DateCreation = DateTime.Now;

            if (!string.IsNullOrEmpty(Birthday))
                Age = CalculateAge(DateTime.Parse(Birthday));

            if (savesId == null)
                SavesId = new List<string>();
            else
                SavesId = savesId;

            var hash = new Hash128();
            hash.Append(Name);
            hash.Append(OwnerName);
            hash.Append(Locality);
            hash.Append(Birthday.ToString());
            hash.Append(DateTime.Now.ToString());
            Id = hash.ToString();
        }

        [JsonConstructor]
        public HorseData(string id, string name, string sex, string birthday, string description, string ownerName, string locality, string phoneNumber, List<string> savesId)
        {
            Id = id;
            Name = name;
            Sex = sex;
            Birthday = birthday;
            Description = description;

            OwnerName = ownerName;
            Locality = locality;
            PhoneNumber = phoneNumber;

            DateCreation = DateTime.Now;

            if (!string.IsNullOrEmpty(Birthday))
                Age = CalculateAge(DateTime.Parse(Birthday));

            if (savesId == null)
                SavesId = new List<string>();
            else
                SavesId = savesId;
        }

        public static int CalculateAge(DateTime birthday)
        {
            var subTicks = DateTime.Today.Ticks - birthday.Ticks;
            DateTime subDate = new DateTime(subTicks);
            return subDate.Year;
        }
    }
}