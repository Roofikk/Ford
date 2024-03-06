using System;
using System.Collections.Generic;

namespace Ford.SaveSystem.Ver2.Dto
{
    public class CreateSaveDto
    {
        public string Header { get; set; } = null!;
        public string Description { get; set; }
        public DateTime Date { get; set; }

        public ICollection<Data.BoneData> Bones { get; set; } = null!;
    }
}
