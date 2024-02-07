using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;

public class Storage
{
    private string _pathSave;

    public string PathSave => _pathSave;
    public string PathHorse => _pathSave + "/Horses/";
    public string PathHorseSaves => _pathSave + "/HorseSaves/";
    private string DevPathSave => _pathSave + "/DevSaves/";

    private DirectoryInfo _horseDirectory;
    private DirectoryInfo _horseSavesDirectory;

    public Storage(string path)
    {
        _pathSave = path;

        if (!CheckHorseDirectory())
            CreateHorseDirectory();

        if (!CheckHorseSaveDirectory())
            CreateHorseSaveDirectory();
    }

    private bool CheckHorseDirectory()
    {
        DirectoryInfo directoryInfo = new(PathHorse);

        if (directoryInfo.Exists)
            _horseDirectory = directoryInfo;

        return directoryInfo.Exists;
    }

    private void CreateHorseDirectory()
    {
        DirectoryInfo directoryInfo = new(PathHorse);
        directoryInfo.Create();
        _horseDirectory = directoryInfo;
    }

    private bool CheckHorseSaveDirectory()
    {
        DirectoryInfo directoryInfo = new(PathHorseSaves);

        if (directoryInfo.Exists)
            _horseSavesDirectory = directoryInfo;

        return directoryInfo.Exists;
    }

    private void CreateHorseSaveDirectory()
    {
        DirectoryInfo directoryInfo = new(PathHorseSaves);
        directoryInfo.Create();
        _horseSavesDirectory = directoryInfo;
    }

    #region Horse

    public List<HorseData> GetHorses()
    {
        FileInfo[] files = _horseDirectory.GetFiles("*.json");
        List<HorseData> horses = new List<HorseData>();

        foreach (var file in files)
        {
            string fileString = File.ReadAllText(file.FullName);
            HorseData data = JsonConvert.DeserializeObject<HorseData>(fileString);
            horses.Add(data);
        }

        return horses;
    }

    public HorseData GetHorse(string id)
    {
        return GetHorses().Find(h => h.Id == id);
    }

    public HorseData FindHorseBySave(HorseSaveData saveData)
    {
        var horses = GetHorses();
        
        foreach (var horse in horses)
        {
            var findSave = horse.SavesId.Find(x => x == saveData.Id);

            if (findSave != null)
                return horse;
        }

        return null;
    }

    public HorseData AddHorse(HorseData horse)
    {
        if (horse == null)
            throw new Exception("Horse not instance");

        List<HorseData> horses = GetHorses();

        if (horses.Find(h => h.Id == horse.Id) != null)
            throw new Exception("Current horse exitsts");

        string data = JsonConvert.SerializeObject(horse);

        StreamWriter sw = new(PathHorse + horse.Id + ".json");
        sw.Write(data);
        sw.Close();

        return horse;
    }

    public HorseData UpdateHorse(HorseData horse)
    {
        List<HorseData> horses = GetHorses();
        HorseData foundHorse = horses.Find(h => h.Id == horse.Id);

        if (foundHorse == null)
            throw new Exception("Horse not found");

        string fullPath = PathHorse + horse.PathSave;
        string stringData = JsonConvert.SerializeObject(horse);
        StreamWriter sw = new StreamWriter(fullPath);
        sw.Write(stringData);
        sw.Close();

        return horse;
    }

    public void DeleteHorse(HorseData horse)
    {
        DeleteHorse(horse.Id);
    }

    public void DeleteHorse(string id)
    {
        HorseData horse = GetHorse(id);

        if (horse == null)
            throw new Exception("Horse not found");

        List<HorseSaveData> saves = GetHorseSaves(horse);

        foreach (var save in saves)
        {
            DeleteHorseSave(save);
        }

        string fullPath = PathHorse + horse.PathSave;
        File.Delete(fullPath);
    }

    #endregion

    #region HorseSave

    public List<HorseSaveData> GetHorseSaves(HorseData horse)
    {
        List<HorseSaveData> horseHistories = new();

        foreach (var path in horse.SavesId)
        {
            string pathSave = PathHorseSaves + path + ".json";

            if (!File.Exists(pathSave))
            {
                Debug.Log($"File {pathSave} is not exsists");
                continue;
            }

            string fileString = File.ReadAllText(pathSave);
            HorseSaveData data = JsonConvert.DeserializeObject<HorseSaveData>(fileString);
            horseHistories.Add(data);
        }

        return horseHistories;
    }

    public HorseSaveData AddHorseSave(HorseSaveData save)
    {
        List<HorseData> horses = GetHorses();
        HorseData horse = horses.Find(h => h.Id == save.HorseId);

        if (horse == null)
            throw new Exception("Horse not found");

        string pathSave = PathHorseSaves + save.Id + ".json";

        if (File.Exists(pathSave))
        {
            Debug.LogError("File is exists. Change the save path");
            return null;
        }

        string stringData = JsonConvert.SerializeObject(save, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });

        StreamWriter sw = new StreamWriter(pathSave);
        sw.Write(stringData);
        sw.Close();

        var findSave = horse.SavesId.Find((saveId) => saveId == save.Id);
        
        if (findSave == null)
            horse.SavesId.Add(save.Id);

        UpdateHorse(horse);

        return save;
    }

    public HorseSaveData UpdateHorseSave(HorseSaveData save)
    {
        List<HorseData> horses = GetHorses();
        HorseData horse = horses.Find(h => h.Id == save.HorseId);

        if (horse == null)
            throw new Exception("Horse not found");

        string fullPathSave = PathHorseSaves + save.Id + ".json";

        if (!File.Exists(fullPathSave))
            throw new Exception("Save file not found");

        string stringData = JsonConvert.SerializeObject(save, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });

        StreamWriter sw = new(fullPathSave);
        sw.Write(stringData);
        sw.Close();

        return save;
    }

    public void DeleteHorseSave(HorseSaveData save)
    {
        string fullPath = PathHorseSaves + save.PathSave;

        if (!File.Exists(fullPath))
            throw new Exception("Save not found");

        File.Delete(fullPath);

        HorseData horseData = FindHorseBySave(save);
        
        if (horseData == null)
            return;

        horseData.SavesId.Remove(save.Id);
        UpdateHorse(horseData);
    }
    #endregion

    #region Dev

    public void DevSaveStateHorse(DevHorseSaveData saveData)
    {
        DirectoryInfo dirInfo = new(DevPathSave);
        if (!dirInfo.Exists)
        {
            dirInfo.Create();
        }

        string pathSave = DevPathSave + "developer_save.json";

        string stringData = JsonConvert.SerializeObject(saveData, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });

        StreamWriter sw = new StreamWriter(pathSave);
        sw.Write(stringData);
        sw.Close();
    }

    public DevHorseSaveData DevGetHorseState(string id)
    {
        DirectoryInfo dirInfo = new(DevPathSave);

        if (!dirInfo.Exists)
        {
            dirInfo.Create();
        }

        string pathSave = DevPathSave + "developer_save.json";

        if (!File.Exists(pathSave))
        {
            Debug.LogWarning("File not exists. Need first save to create file");
            return null;
        }

        string fileString = File.ReadAllText(pathSave);
        DevHorseSaveData data = JsonConvert.DeserializeObject<DevHorseSaveData>(fileString);

        return data;
    }
    #endregion
}