using System;
using System.Collections.Generic;

namespace Ford.SaveSystem.Ver2.Dto
{
    public class CreationHorseData
    {
        public string Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Sex { get; set; } = null!;
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string OwnerName { get; set; }
        public string PhoneNumber { get; set; }
        public ICollection<RequestHorseUserDto> Users { get; set; }
    }

    public class RequestHorseUserDto
    {
        public long UserId { get; set; }
        public string RuleAccess { get; set; }
        public bool IsOwner { get; set; }
    } 
}
