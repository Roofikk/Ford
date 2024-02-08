using Ford.SaveSystem.Ver2;
using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class ForTestScene : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _resultText;

    [Header("Buttons")]
    [SerializeField] private Button _getHorseButton;
    [SerializeField] private Button _getHorsesButton;
    [SerializeField] private Button _createHorseButton;

    private string _pathToSave;
    Ford.SaveSystem.Ver2.Storage _storage;

    private void Start()
    {
        _pathToSave = Application.persistentDataPath;
        _storage = new();

        _getHorsesButton.onClick.AddListener(GetHorses);
        _createHorseButton.onClick.AddListener(CreateHorse);
    }

    private void GetHorses()
    {
        ClearText();
        var horses = _storage.GetHorses();
        _resultText.text = JsonConvert.SerializeObject(horses, Formatting.Indented);
    }

    private void CreateHorse()
    {
        ClearText();
        var id = Guid.NewGuid().ToString();
        HorseLocalSaveData horseLocalSaveData = new HorseLocalSaveData()
        {
            Id = id,
            Name = id,
            Description = id,
            Sex = "Male",
            BirthDate = DateTime.Now,
            City = "There",
            Region = "There there",
            Country = "Russia",
            DateCreation = DateTime.Now,
            Saves = Array.Empty<LocalSaveData>()
        };

        var createdHorse = _storage.CreateHorse(horseLocalSaveData);
        _resultText.text = JsonConvert.SerializeObject(createdHorse, Formatting.Indented);
    }

    private void ClearText()
    {
        _resultText.text = string.Empty;
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
