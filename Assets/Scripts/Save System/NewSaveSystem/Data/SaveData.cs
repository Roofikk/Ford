using System;

namespace Ford.SaveSystem
{
    public class SaveData : ISaveData
    {
        public long Id { get; set; }
        public string Header { get; set; } = null!;
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public string SaveFileName { get; set; } = null!;
    }
}
