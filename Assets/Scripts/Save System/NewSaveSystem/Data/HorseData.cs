using System;
using System.Collections.Generic;

namespace Ford.SaveSystem.Ver2.Data
{
    public class HorseData
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Sex { get; set; } = null!;
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime LastUpdate { get; set; }

        public HorseOwnerData Owner { get; set; }

        public ICollection<SaveData> Saves { get; set; }
    }
}
