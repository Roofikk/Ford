using Ford.SaveSystem.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Ford.SaveSystem.Data.Dtos
{
    public class CreatingSaveDto
    {
        public long? HorseId { get; set; }
        public string Header { get; set; }
        public string Description { get; set; }
        [JsonConverter(typeof(DateConverter))]
        public DateTime Date { get; set; }
        public DateTime? CreationDate { get; set; }
        public List<BoneSave> Bones { get; set; } = new();
    }
}
