using Ford.SaveSystem.Data;
using Ford.SaveSystem.Ver2.Data;
using Ford.SaveSystem.Ver2.Dto;
using Ford.WebApi.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Ford.SaveSystem.Ver2
{
    public class Storage
    {
        private string _storagePath;
        private string _savesPath;

        private readonly string _horsesFileName = "horses.json";
        private readonly string _storageSettingsFileName = "storageSettings.json";

        private readonly string ACCESS_TOKEN_KEY = "ACCESS_TOKEN_KEY";
        private readonly string REFRESH_TOKEN_KEY = "REFRESH_TOKEN_KEY";

        private List<HorseBase> _horses;

        public static StorageHistory History { get; private set; }

        public Storage()
        {
            _horses = new();
            _storagePath = Path.Combine(Environment.CurrentDirectory, "storage");
            _savesPath = Path.Combine(_storagePath, "saves");

            if (!Directory.Exists(_savesPath))
            {
                Directory.CreateDirectory(_savesPath);
            }

            History ??= new(_storagePath);
        }

        public Storage(string storagePath)
        {
            _horses = new();
            _storagePath = storagePath;
            _savesPath = Path.Combine(_storagePath, "saves");

            if (!Directory.Exists(_savesPath))
            {
                Directory.CreateDirectory(_savesPath);
            }
        }

        #region Horse CRUD
        public ICollection<HorseBase> GetHorses()
        {
            if (_horses.Count > 0)
            {
                return _horses;
            }

            string pathHorses = Path.Combine(_storagePath, _horsesFileName);

            if (!File.Exists(pathHorses))
            {
                return null;
            }

            ICollection<HorseBase> horseData = GetSerializableArrayFromFile<HorseBase>(pathHorses);
            using (StreamReader sr = new(pathHorses))
            using (JsonTextReader reader = new(sr))
            {
                reader.SupportMultipleContent = true;
                var serializer = new JsonSerializer();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        horseData = serializer.Deserialize<ArraySerializable<HorseBase>>(reader)?.Items;
                    }
                }
            }

            _horses = horseData.ToList();
            return _horses;
        }

        public HorseBase GetHorse(long horseId)
        {
            var horses = GetHorses();

            if (horses == null)
            {
                return null;
            }

            var findHorse = horses.FirstOrDefault(h => h.HorseId == horseId);
            return findHorse;
        }

        public HorseBase CreateHorse(CreationHorse horseData)
        {
            ICollection<HorseBase> horses = GetHorses();
            horses ??= new Collection<HorseBase>();

            var horseId = IncrementHorseId();

            HorseBase addHorse = new()
            {
                HorseId = horseId,
                Name = horseData.Name,
                Description = horseData.Description,
                BirthDate = horseData.BirthDate,
                Sex = horseData.Sex,
                City = horseData.City,
                Region = horseData.Region,
                Country = horseData.Country,
                CreationDate = DateTime.Now,
                LastUpdate = DateTime.Now,
                Saves = new List<SaveInfo>(),
            };

            if (!string.IsNullOrEmpty(horseData.OwnerName))
            {
                addHorse.OwnerName = horseData.OwnerName;
                addHorse.OwnerPhoneNumber = horseData.OwnerPhoneNumber;
            }

            horses.Add(addHorse);
            RewriteHorseFile(horses);
            History.PushHistory(new(ActionType.CreateHorse, addHorse));

            return addHorse;
        }

        public HorseBase UpdateHorse(UpdatingHorse horseData)
        {
            var horses = GetHorses();

            if (horses is null)
            {
                return null;
            }

            HorseBase existHorse = horses.FirstOrDefault(h => h.HorseId == horseData.HorseId);

            if (existHorse is null)
            {
                return null;
            }

            //
            existHorse.Name = horseData.Name;
            existHorse.Description = horseData.Description;
            existHorse.BirthDate = horseData.BirthDate;
            existHorse.Sex = horseData.Sex;
            existHorse.City = horseData.City;
            existHorse.Region = horseData.Region;
            existHorse.Country = horseData.Country;
            existHorse.LastUpdate = DateTime.Now;
            //

            RewriteHorseFile(horses);
            History.PushHistory(new(ActionType.UpdateHorse, existHorse));

            return existHorse;
        }

        // не забыть удалить сохранения лошади, поскольку они не нужны.
        public bool DeleteHorse(long id)
        {
            var horses = GetHorses();

            if (horses == null)
            {
                return false;
            }

            var findHorse = horses.FirstOrDefault(h => h.HorseId == id);
            var query = findHorse.Saves.GroupBy(s => s.SaveFileName).Select(q => new { FileName = q.Key, Ids = q.Select(id => id.SaveId)});

            foreach (var path in query)
            {
                DeleteSaves(Path.Combine(_savesPath, path.FileName), path.Ids.ToArray());
            }

            bool result = horses.Remove(findHorse);

            if (!result)
            {
                return false;
            }

            RewriteHorseFile(horses);
            History.PushHistory(new(ActionType.DeleteHorse, new HorseBase() { HorseId = id }));

            return true;
        }
        #endregion

        #region Save CRUD
        public ICollection<SaveInfo> GetSaves(long horseId, int below = 0, int amount = 20)
        {
            var horse = GetHorse(horseId);
            return horse.Saves
                .Skip(below)
                .Take(amount)
                .ToList();
        }

        public FullSaveInfo GetFullSave(long horseId, long saveId)
        {
            var savesInfo = GetHorse(horseId).Saves;
            var saveInfo = savesInfo.SingleOrDefault(s => s.SaveId == saveId);

            if (saveInfo == null)
            {
                return null;
            }

            var saveBones = GetSave(saveInfo.SaveFileName, saveId);
            FullSaveInfo fullSaveInfo = (FullSaveInfo)saveInfo;
            fullSaveInfo.Bones = saveBones.Bones;
            return fullSaveInfo;
        }

        public ICollection<SaveBonesData> GetSaves(string fileName)
        {
            string savePath = Path.Combine(_savesPath, fileName);

            if (!File.Exists(savePath))
            {
                return null;
            }

            return GetSerializableArrayFromFile<SaveBonesData>(savePath);
        }

        public SaveBonesData GetSave(string fileName, long saveId)
        {
            List<SaveBonesData> saves = GetSaves(fileName).ToList();

            if (saves is null)
            {
                return null;
            }

            return saves.FirstOrDefault(s => s.SaveId == saveId);
        }

        public SaveInfo CreateSave(FullSaveInfo saveData)
        {
            var horses = GetHorses();

            if (horses is null)
            {
                return null;
            }

            HorseBase existingHorse = horses.FirstOrDefault(h => h.HorseId == saveData.HorseId);

            if (existingHorse is null)
            {
                return null;
            }

            string fileName = GetSaveFileName();
            existingHorse.Saves ??= new Collection<SaveInfo>();

            long saveId = IncrementSaveId();
            SaveInfo save = new()
            {
                SaveId = saveId,
                Header = saveData.Header,
                Description = saveData.Description,
                Date = saveData.Date,
                CreationDate = DateTime.Now,
                LastUpdate = DateTime.Now,
                SaveFileName = fileName
            };

            existingHorse.Saves.Add(save);
            RewriteHorseFile(horses);

            string pathSave = Path.Combine(_savesPath, fileName);
            SaveBonesData saveBones = new()
            {
                SaveId = saveId,
                Bones = saveData.Bones
            };

            var saves = GetSaves(pathSave);

            if (saves is null)
            {
                saves = new List<SaveBonesData>() { saveBones };
            }
            else
            {
                saves.Add(saveBones);
            }

            RewriteSaveBonesFile(pathSave, saves);
            History.PushHistory(new(ActionType.CreateSave, save));

            return save;
        }

        public SaveInfo UpdateSave(SaveInfo saveData)
        {
            var horses = GetHorses();

            if (horses is null)
            {
                return null;
            }

            var save = horses.SelectMany(h => h.Saves).FirstOrDefault(s => s.SaveId == saveData.SaveId);

            if (save is null)
            {
                return null;
            }

            save.Header = saveData.Header;
            save.Description = saveData.Description;
            save.Date = saveData.Date;
            save.LastUpdate = DateTime.Now;

            RewriteHorseFile(horses);
            History.PushHistory(new(ActionType.UpdateSave, save));
            return save;
        }

        public bool DeleteSave(long saveId)
        {
            var horses = GetHorses();

            if (horses is null)
            {
                return false;
            }

            var savesInfo = horses.FirstOrDefault(h => h.Saves.Any(s => s.SaveId == saveId)).Saves;
            var saveInfo = savesInfo.FirstOrDefault(s => s.SaveId == saveId);
            var saves = GetSaves(saveInfo.SaveFileName);

            if (saves is null)
            {
                return false;
            }

            var saveData = saves.FirstOrDefault(s => s.SaveId == saveId);

            saves.Remove(saveData);
            RewriteSaveBonesFile(saveInfo.SaveFileName, saves);

            savesInfo!.Remove(saveInfo);
            RewriteHorseFile(horses);
            History.PushHistory(new(ActionType.DeleteSave, new SaveInfo() { SaveId = saveId }));

            return true;
        }

        private void DeleteSaves(string pathSave, long[] saveIds)
        {
            var saves = GetSerializableArrayFromFile<SaveBonesData>(pathSave).ToList() ?? throw new Exception($"Saves not serialized is {pathSave} file");

            saves.RemoveAll(s => saveIds.Contains(s.SaveId));
            RewriteSaveBonesFile(pathSave, saves);
        }
        #endregion

        public string GetAccessToken()
        {
            return PlayerPrefs.GetString(ACCESS_TOKEN_KEY, "");
        }

        public string GetRefreshToken()
        {
            return PlayerPrefs.GetString(REFRESH_TOKEN_KEY, "");
        }

        public void SaveAccessToken(string accessToken)
        {
            PlayerPrefs.SetString(ACCESS_TOKEN_KEY, accessToken);
            PlayerPrefs.Save();
        }

        public void SaveRefreshToken(string refreshToken)
        {
            PlayerPrefs.SetString(REFRESH_TOKEN_KEY, refreshToken);
            PlayerPrefs.Save();
        }

        public void ClearAccessToken()
        {
            PlayerPrefs.SetString(ACCESS_TOKEN_KEY, "");
            PlayerPrefs.Save();
        }

        public void ClearRefreshToken()
        {
            PlayerPrefs.SetString(REFRESH_TOKEN_KEY, "");
            PlayerPrefs.Save();
        }

        private string GetSaveFileName()
        {
            string pathSettings = Path.Combine(_storagePath, _storageSettingsFileName);
            string fileName = Guid.NewGuid().ToString() + ".json";

            if (File.Exists(pathSettings))
            {
                using StreamReader sr = new StreamReader(pathSettings);
                using JsonReader reader = new JsonTextReader(sr);

                while (reader.Read())
                {
                    reader.SupportMultipleContent = true;
                    var serializer = new JsonSerializer();
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        var settings = serializer.Deserialize<StorageSettingsData>(reader) ?? throw new Exception("File not exists");
                        fileName = settings.LastSaveFileName;
                    }
                }

                FileInfo saveFile = new FileInfo(Path.Combine(_savesPath, fileName));
                float fileSizeMb = saveFile.Length / (1024f * 1024f);

                if (saveFile.Length < 10f)
                {
                    return fileName;
                }
            }

            using StreamWriter sw = new StreamWriter(pathSettings);
            using JsonWriter jsonWriter = new JsonTextWriter(sw);
            JsonSerializer.CreateDefault().Serialize(jsonWriter, new StorageSettingsData()
            {
                LastSaveFileName = fileName,
                IncrementSave = 0,
                IncrementHorse = 0,
            });

            return fileName;
        }

        private ICollection<T> GetSerializableArrayFromFile<T>(string path)
        {
            ICollection<T> collection = null;

            using (StreamReader sr = new(path))
            using (JsonTextReader reader = new(sr))
            {
                reader.SupportMultipleContent = true;
                var serializer = new JsonSerializer();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        collection = serializer.Deserialize<ArraySerializable<T>>(reader)?.Items;
                    }
                }
            }

            return collection;
        }

        private void RewriteHorseFile(ICollection<HorseBase> horses)
        {
            string path = Path.Combine(_storagePath, _horsesFileName);
            using StreamWriter sw = new(path);
            using JsonWriter jsonWriter = new JsonTextWriter(sw);
            JsonSerializer.CreateDefault().Serialize(jsonWriter, new ArraySerializable<HorseBase>(horses));
        }

        private void RewriteSaveBonesFile(string path, ICollection<SaveBonesData> saves)
        {
            if (saves.Count() == 0)
            {
                File.Delete(path);
                return;
            }

            using StreamWriter sw = new(path);
            using JsonWriter jsonWriter = new JsonTextWriter(sw);
            JsonSerializer.CreateDefault().Serialize(jsonWriter, new ArraySerializable<SaveBonesData>(saves));
        }

        private long IncrementSaveId()
        {
            string pathSettings = Path.Combine(_storagePath, _storageSettingsFileName);
            long inc = 0;
            StorageSettingsData settings = null;

            if (File.Exists(pathSettings))
            {
                using StreamReader sr = new StreamReader(pathSettings);
                using JsonReader reader = new JsonTextReader(sr);

                while (reader.Read())
                {
                    reader.SupportMultipleContent = true;
                    var serializer = new JsonSerializer();
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        settings = serializer.Deserialize<StorageSettingsData>(reader) ?? throw new Exception("File not exists");
                        inc = ++settings.IncrementSave;
                    }
                }
            }

            using StreamWriter sw = new StreamWriter(pathSettings);
            using JsonWriter jsonWriter = new JsonTextWriter(sw);

            settings ??= new StorageSettingsData()
            {
                LastSaveFileName = Guid.NewGuid().ToString(),
                IncrementSave = inc,
                IncrementHorse = 0
            };

            JsonSerializer.CreateDefault().Serialize(jsonWriter, new StorageSettingsData()
            {
                LastSaveFileName = settings.LastSaveFileName,
                IncrementSave = inc,
                IncrementHorse = settings.IncrementHorse,
            });

            return inc;
        }

        private long IncrementHorseId()
        {
            string pathSettings = Path.Combine(_storagePath, _storageSettingsFileName);
            long inc = 0;
            StorageSettingsData settings = null;

            if (File.Exists(pathSettings))
            {
                using StreamReader sr = new StreamReader(pathSettings);
                using JsonReader reader = new JsonTextReader(sr);

                while (reader.Read())
                {
                    reader.SupportMultipleContent = true;
                    var serializer = new JsonSerializer();
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        settings = serializer.Deserialize<StorageSettingsData>(reader) ?? throw new Exception("File not exists");
                        inc = ++settings.IncrementHorse;
                    }
                }
            }

            using StreamWriter sw = new StreamWriter(pathSettings);
            using JsonWriter jsonWriter = new JsonTextWriter(sw);

            settings ??= new StorageSettingsData()
            {
                LastSaveFileName = Guid.NewGuid().ToString(),
                IncrementSave = 0,
                IncrementHorse = inc
            };

            JsonSerializer.CreateDefault().Serialize(jsonWriter, new StorageSettingsData()
            {
                LastSaveFileName = settings.LastSaveFileName,
                IncrementSave = settings.IncrementSave,
                IncrementHorse = inc,
            });

            return inc;
        }
    }
}
