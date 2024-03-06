using System.Collections.Generic;

namespace Ford.SaveSystem.Ver2.Data
{
    public class SaveBonesData
    {
        public long SaveId { get; set; }
        public ICollection<BoneData> Bones { get; set; } = null!;
    }
}
