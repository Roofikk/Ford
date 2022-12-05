using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;

public class Storage
{
    private string _pathSave;
    public string PathHorse => _pathSave + "Horses/";
    public string PathHorseSaves => _pathSave + "HorseSaves/";

    public Storage()
    {
        _pathSave = Application.persistentDataPath + "Saves/";
    }

    #region Horse

    public List<HorseData> GetHorses()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(PathHorse);

        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
            return new List<HorseData>();
        }

        FileInfo[] files = directoryInfo.GetFiles("*.json");
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

    public HorseData AddHorse(HorseData horse)
    {
        if (horse == null)
            throw new Exception("Horse not instance");

        List<HorseData> horses = GetHorses();

        if (horses.Find(h => h.Id == horse.Id) != null)
            throw new Exception("Current horse exitsts");

        string data = JsonConvert.SerializeObject(horse);
        StreamWriter sw = new StreamWriter(PathHorse + horse.Id + ".json");
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

        string fullPath = PathHorse + horse.PathSave;
        File.Delete(fullPath);

        List<HorseSaveData> saves = GetHorseSaves(horse);

        foreach (var save in saves)
        {
            fullPath = PathHorseSaves + save.PathSave;
            File.Delete(fullPath);
        }
    }

    #endregion

    public List<HorseSaveData> GetHorseSaves(HorseData horse)
    {
        List<HorseSaveData> horseHistories = new List<HorseSaveData>();

        foreach (var path in horse.SavesId)
        {
            string pathSave = PathHorseSaves + path;

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
            Debug.LogError("File is exists. Change the path save");
            return null;
        }

        string stringData = JsonConvert.SerializeObject(save);
        StreamWriter sw = new StreamWriter(pathSave);
        sw.Write(stringData);
        sw.Close();

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

        string stringData = JsonConvert.SerializeObject(save);
        StreamWriter sw = new StreamWriter(fullPathSave);
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
    }
}