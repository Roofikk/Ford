using Ford.SaveSystem.Ver2;
using System;

namespace Ford.SaveSystem
{
    public class SaveInfo : ISaveInfo, IStorageData
    {
        public long SaveId { get; set; }
        public long HorseId { get; set; }
        public string Header { get; set; } = null!;
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public string SaveFileName { get; set; } = null!;

        public string ActionDescription => $"Заголовок: {Header}\nОписание: {Description}\nДата: {Date:dd.MM.yyyy}";

        public SaveInfo(SaveInfo saveData)
        {
            SaveId = saveData.SaveId;
            HorseId = saveData.HorseId;
            Header = saveData.Header;
            Description = saveData.Description;
            Date = saveData.Date;
            CreationDate = saveData.CreationDate;
            LastUpdate = saveData.LastUpdate;
            SaveFileName = saveData.SaveFileName;
        }

        public SaveInfo()
        {
        }
    }
}
