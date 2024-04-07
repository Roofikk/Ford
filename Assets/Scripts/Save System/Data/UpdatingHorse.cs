using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Ford.SaveSystem.Data
{
    public class UpdatingHorse
    {
        public long HorseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonConverter(typeof(DateConverter))]
        public DateTime? BirthDate { get; set; }
        public string Sex { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string OwnerName { get; set; }
        public string OwnerPhoneNumber { get; set; }
        public ICollection<CreationHorseUser> Users { get; set; } = new List<CreationHorseUser>();
    }
}
