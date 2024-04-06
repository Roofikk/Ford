using Newtonsoft.Json;

namespace Ford.SaveSystem.Ver2
{
    public interface IStorageData
    {
        public long HorseId { get; set; }
        [JsonIgnore]
        public string ActionDescription { get; }
    }
}
