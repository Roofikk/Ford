using System;
using System.Collections.Generic;

namespace Ford.WebApi.Data
{
    public class HorseRetrieveDto
    {
        public long HorseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Sex { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public DateTime CreationDate { get; set; }
        public IEnumerable<HorseUserDto> Users { get; set; }
    }
}
