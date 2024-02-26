using System.Collections.Generic;

namespace Ford.SaveSystem.Ver2.Data
{
    public class SaveBonesData
    {
        public string SaveId { get; set; } = null!;
        public ICollection<BoneData> Bones { get; set; } = null!;
    }
}
