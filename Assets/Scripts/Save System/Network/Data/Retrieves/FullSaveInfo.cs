using Ford.WebApi.Data;
using System.Collections.Generic;

namespace Ford.SaveSystem
{
    public class FullSaveInfo : SaveInfo
    {
        public ICollection<BoneSave> Bones { get; set; }
    }
}
