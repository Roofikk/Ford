using Newtonsoft.Json;
using System;

namespace Ford.SaveSystem.Data
{
    public interface ISaveInfo
    {
        public long SaveId { get; set; }
        public long HorseId { get; set; }
        public string Header { get; set; }
        public string Description { get; set; }
        [JsonConverter(typeof(DateConverter))]
        public DateTime Date { get; set; }
        public UserDate CreatedBy { get; set; }
        public UserDate LastModifiedBy { get; set; }
    }
}
