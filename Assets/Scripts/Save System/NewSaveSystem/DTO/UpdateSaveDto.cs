using System;

namespace Ford.SaveSystem.Ver2.Dto
{
    public class UpdateSaveDto
    {
        public string Id { get; set; } = null!;
        public string Header { get; set; } = null!;
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}
