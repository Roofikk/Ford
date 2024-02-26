using Ford.SaveSystem.Ver2.Data;
using System.Collections.Generic;

namespace Ford.SaveSystem.Ver2.Dto
{
    public class FullSaveData : CreateSaveDto
    {
        public string FileName { get; set; } = null!;
        public ICollection<BoneData> Bones { get; set; } = null!;
    }
}
