using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HorseData
{
    public string Id { get; private set; }
    public string Name { get; set; }
    public DateTime Birthday { get; set; }
    public string Sex { get; set; }
    public string Description { get; set; }

    public string OwnerName { get; set; }
    public string Locality { get; set; }

    public DateTime DateCreation { get; set; }
    public List<string> SavesId { get; set; }

    [JsonIgnore] public int Age { get; private set; }
    [JsonIgnore] public string PathSave => Id + ".json";

    public HorseData(string name, string sex, DateTime birthday, string description, string ownerName, string locality, List<string> savesId)
    {
        Name = name;
        Sex = sex;
        Birthday = birthday;
        Description = description;

        OwnerName = ownerName;
        Locality = locality;

        DateCreation = DateTime.Now;
        Age = CalculateAge(Birthday);

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
    public HorseData(string id, string name, string sex, DateTime birthday, string description, string ownerName, string locality, List<string> savesId)
    {
        Id = id;
        Name = name;
        Sex = sex;
        Birthday = birthday;
        Description = description;

        OwnerName = ownerName;
        Locality = locality;

        DateCreation = DateTime.Now;
        Age = CalculateAge(Birthday);

        if (savesId == null)
            SavesId = new List<string>();
        else
            SavesId = savesId;
    }

    private int CalculateAge(DateTime birthday)
    {
        var subTicks = DateTime.Today.Ticks - birthday.Ticks;
        DateTime subDate = new DateTime(subTicks);
        return subDate.Year;
    }
}