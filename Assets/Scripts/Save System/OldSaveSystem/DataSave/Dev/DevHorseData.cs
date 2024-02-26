using Ford.SaveSystem.Ver1.Data;
using System.Collections.Generic;

namespace Ford.SaveSystem.Ver1.Data
{
    public class DevHorseData : HorseData
    {
        public DevHorseData(string name, string sex, string birthday, string description, string ownerName, string locality, string phoneNumber, List<string> savesId)
            : base(name, sex, birthday, description, ownerName, locality, phoneNumber, savesId)
        {
            Id = "dev_id";
        }
    }
}
