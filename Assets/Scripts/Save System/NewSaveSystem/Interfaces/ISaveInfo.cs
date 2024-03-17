using System;

namespace Ford.SaveSystem
{
    public interface ISaveInfo
    {
        public long SaveId { get; set; }
        public long HorseId { get; set; }
        public string Header { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
