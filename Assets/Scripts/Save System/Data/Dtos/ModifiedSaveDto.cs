using System;

namespace Ford.SaveSystem.Data.Dtos
{
    public class ModifiedSaveDto
    {
        public long SaveId { get; set; }
        public string Header { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
