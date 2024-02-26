using Ford.SaveSystem.Interfaces;
using System;

namespace Ford.SaveSystem.Ver2.Dto
{
    public class RetrieveSaveData : ISaveData
    {
        public string Id { get; set; } = null!;
        public string Header { get; set; } = null!;
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
