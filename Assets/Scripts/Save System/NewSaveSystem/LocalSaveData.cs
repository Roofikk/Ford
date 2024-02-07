using System;

namespace Ford.SaveSystem.Ver2
{
    public class LocalSaveData
    {
        public string Id { get; set; }
        public string Header { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime LastUpdate{ get; set; }
        public string PathFileSave { get; set; }
    }
}
