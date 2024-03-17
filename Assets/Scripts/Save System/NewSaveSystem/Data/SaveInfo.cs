using Ford.SaveSystem.Ver2;
using System;

namespace Ford.SaveSystem
{
    public class SaveInfo : ISaveInfo, IStorageAction
    {
        public long SaveId { get; set; }
        public long HorseId { get; set; }
        public string Header { get; set; } = null!;
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public string SaveFileName { get; set; } = null!;

        public SaveInfo(SaveInfo saveData)
        {
            SaveId = saveData.SaveId;
            Header = saveData.Header;
            Description = saveData.Description;
            CreationDate = saveData.CreationDate;
            LastUpdate = saveData.LastUpdate;
            SaveFileName = saveData.SaveFileName;
        }

        public SaveInfo()
        {
        }
    }
}
