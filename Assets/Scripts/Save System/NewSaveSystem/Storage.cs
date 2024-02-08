using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

namespace Ford.SaveSystem.Ver2
{
    public class Storage
    {
        private string _pathSave;
        private readonly string horsesFileName = "horses.json";

        public Storage()
        {
            _pathSave = Path.Combine(Application.persistentDataPath, "Saves");
        }

        public Storage(string pathSave)
        {
            _pathSave = pathSave;
        }

        #region Horse CRUD
        // ���������� ����� � �������������� ���������� ����������, � �� ����� �����.
        public HorseLocalSaveData GetHorse(string id)
        {
            string pathHorses = Path.Combine(_pathSave, horsesFileName);

            if (!FileIsExists(pathHorses))
            {
                throw new Exception("Path to horses is incorrect");
            }

            StreamReader sr = new(pathHorses);
            HorseLocalSaveData horseData = null;

            using (JsonTextReader reader = new(sr))
            {
                reader.SupportMultipleContent = true;
                var serializer = new JsonSerializer();
                while (reader.Read())
                {
                    horseData = serializer.Deserialize<HorseLocalSaveData>(reader);

                    if (horseData is not null)
                    {
                        if (horseData.Id == id)
                        {
                            break;
                        }
                    }
                }
            }

            sr.Dispose();
            return horseData;
        }

        public IEnumerable<HorseLocalSaveData> GetHorses()
        {
            string pathHorses = Path.Combine(_pathSave, horsesFileName);

            if (!FileIsExists(pathHorses))
            {
                throw new Exception("Path to horses is incorrect");
            }

            string json = "";

            using (StreamReader sr = new StreamReader(pathHorses))
            {
                json = sr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<ArraySerializable<HorseLocalSaveData>>(json).Items;
        }

        // ���� ������ ���������� ��������� ��� ������� ����� ������� � �����.
        // � ��� ������ ����������� �� �����, ����� �� ���������� ����� ������ � �� ��������� ��� -- ������
        // ����� �� �������� �� �������������� ���� ����, � �������� � ���� json ������.
        public HorseLocalSaveData CreateHorse(HorseLocalSaveData horseData)
        {
            if (string.IsNullOrEmpty(horseData.Id))
            {
                horseData.Id = Guid.NewGuid().ToString();
            }
            else
            {
                if (GetHorse(horseData.Id) is not null)
                {
                    throw new Exception("Horse is already exists");
                }
            }

            string pathHorses = Path.Combine(_pathSave, horsesFileName);

            if (!FileIsExists(pathHorses))
            {
                throw new Exception("Path to horses is incorrect");
            }

            //StreamWriter sw = new(pathHorses, true);
            //sw.Re
            //using (JsonTextWriter jtw = new(sw))
            //{
            //    jtw.WriteStartObject();

            //    jtw.Se
            //    jtw.Formatting = Formatting.None;
            //    jtw.WriteStartObject();

            //}
            
            //sw.Dispose();
            return horseData;
        }
        #endregion

        private bool FileIsExists(string path) => File.Exists(path);
    }
}
