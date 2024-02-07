using Ford.SaveSystem.Ver2;
using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

public class HorseLocalSaveFab : MonoBehaviour
{
    private string _pathToSave = Application.persistentDataPath;

    private void Start()
    {
        //CreateTestHorses();
        List<HorseLocalSaveData> horses = DeserializeFile();

        Task check = new Task(() => { CheckUniqueIdInHorsesData(horses); });
        check.RunSynchronously();
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
        StreamReader sr = new StreamReader(_pathToSave);
        string json = sr.ReadToEnd();
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

        return true;
    }
}
