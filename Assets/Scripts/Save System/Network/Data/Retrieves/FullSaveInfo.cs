using Ford.SaveSystem.Ver2;
using Ford.WebApi.Data;
using System.Collections.Generic;

namespace Ford.SaveSystem
{
    public class FullSaveInfo : SaveInfo, IStorageData
    {
        public ICollection<BoneSave> Bones { get; }

        public FullSaveInfo()
        {
            Bones = new List<BoneSave>();
        }

        public FullSaveInfo(SaveInfo saveInfo) : base(saveInfo)
        {
            Bones = new List<BoneSave>();
        }
    }
}
