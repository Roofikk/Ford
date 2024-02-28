using System;
using System.Collections.Generic;

namespace Ford.WebApi.Data
{
    public class CreationHorseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Sex { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public IEnumerable<CreationHorseOwner> HorseOwners { get; set; }
    }

    public class CreationHorseOwner
    {
        public string UserId { get; set; }
        public string RuleAccess { get; set; }
    }
}
