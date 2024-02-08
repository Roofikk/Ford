using Ford.SaveSystem.Ver2;
using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

public class HorseLocalSaveFab : MonoBehaviour
{
    private string _pathToSave;

    private void Start()
    {
        _pathToSave = Application.persistentDataPath;

        //CreateTestHorses();
        CreateTestHorses();
        List<HorseLocalSaveData> horses = DeserializeFile();
    }

    private void CreateTestHorses()
    {
        List<HorseLocalSaveData> horseList = new List<HorseLocalSaveData>();

        for (int i = 0; i < 500; i++)
        {
            List<LocalSaveData> saveDataList = new List<LocalSaveData>();

            for (int j = 0; j < 500; j++)
            {
                saveDataList.Add(new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Header = $"Horse - {i}, save - {j}",
                    Description = $"Description for horse - {i}, save - {j}",
                    DateTime = DateTime.Now,
                    LastUpdate = DateTime.Now,
                    PathFileSave = _pathToSave
                });
            }

            horseList.Add(new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = $"Horse - {i}",
                Description = $"Description for horse - {i}",
                Sex = "Female",
                Region = $"Region - {i}",
                City = $"City - {i}",
                Country = $"Russia - {i}",
                BirthDate = DateTime.Now,
                DateCreation = DateTime.Now,
                Saves = saveDataList,
            });
        }

        string json = JsonConvert.SerializeObject(new ArraySerializable<HorseLocalSaveData>(horseList), Formatting.None);

        using (StreamWriter sw = new StreamWriter($"{_pathToSave}\\save.json"))
        {
            sw.Write(json);
        }

        Debug.Log("Save success. Path:");
        Debug.Log(_pathToSave);
    }

    private List<HorseLocalSaveData> DeserializeFile()
    {
        StreamReader sr = new StreamReader(_pathToSave + "\\save.json");
        string json = sr.ReadToEnd();
        sr.Close();
        sr.Dispose();

        var horses = JsonConvert.DeserializeObject<ArraySerializable<HorseLocalSaveData>>(json);
        return new List<HorseLocalSaveData>(horses.Items);
    }

    private bool CheckUniqueIdInHorsesData(List<HorseLocalSaveData> horses)
    {
        List<string> IDs = new List<string>();

        foreach (var horse in horses)
        {
            IDs.Add(horse.Id);

            foreach (var save in horse.Saves)
            {
                IDs.Add(save.Id);
            }
        }

        var uniqueIds = IDs.Distinct();

        return uniqueIds.Count() == IDs.Count;
    }
}
