using System;
using System.Collections.Generic;

namespace Ford.WebApi.Data
{
    public class FullSaveInfo : ISaveInfo
    {
        public long SaveId { get; set; }
        public long HorseId { get; set; }
        public string Header { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public IEnumerable<BoneSave> Bones { get; set; }
    }
}
