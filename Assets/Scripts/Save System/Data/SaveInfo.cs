using Ford.SaveSystem.Ver2;
using Newtonsoft.Json;
using System;

namespace Ford.SaveSystem.Data
{
    public class SaveInfo : ISaveInfo, IStorageData
    {
        public long SaveId { get; set; }
        public long HorseId { get; set; }
        public string Header { get; set; } = null!;
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public UserDate CreatedBy { get; set; }
        public UserDate LastModifiedBy { get; set; }
        public string SaveFileName { get; set; } = null!;

        [JsonIgnore]
        public string ActionDescription => $"Заголовок: {Header}\nОписание: {Description}\nДата: {Date:dd.MM.yyyy}";

        public SaveInfo(SaveInfo saveData)
        {
            SaveId = saveData.SaveId;
            HorseId = saveData.HorseId;
            Header = saveData.Header;
            Description = saveData.Description;
            Date = saveData.Date;
            CreatedBy = saveData.CreatedBy;
            LastModifiedBy = saveData.LastModifiedBy;
            SaveFileName = saveData.SaveFileName;
        }

        public SaveInfo()
        {
        }
    }
}
