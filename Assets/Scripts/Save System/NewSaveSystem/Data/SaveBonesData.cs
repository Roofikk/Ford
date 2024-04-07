using Ford.SaveSystem.Data;
using System.Collections.Generic;

namespace Ford.SaveSystem.Ver2.Data
{
    public class SaveBonesData
    {
        public long SaveId { get; set; }
        public ICollection<BoneSave> Bones { get; } = new List<BoneSave>();
    }
}
