using System;

namespace Ford.SaveSystem.Ver2.Dto
{
    public class UpdateSaveDto
    {
        public long Id { get; set; }
        public string Header { get; set; } = null!;
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}
