using Ford.WebApi.Data;
using System;
using System.Collections.Generic;

namespace Ford.SaveSystem
{
    public class HorseBase
    {
        public long HorseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Sex { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string OwnerName { get; set; }
        public string OwnerPhoneNumber { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public ICollection<HorseUserDto> Users { get; set; }
        public ICollection<SaveData> Saves { get; set; }

        public HorseBase(HorseBase horse)
        {
            HorseId = horse.HorseId;
            Name = horse.Name;
            Description = horse.Description;
            BirthDate = horse.BirthDate;
            Sex = horse.Sex;
            City = horse.City;
            Region = horse.Region;
            Country = horse.Country;
            OwnerName = horse.OwnerName;
            OwnerPhoneNumber = horse.OwnerPhoneNumber;
            CreationDate = horse.CreationDate;
            LastUpdate = horse.LastUpdate;
            Users = horse.Users;
            Saves = horse.Saves;
        }

        public HorseBase()
        {
        }
    }

    public enum HorseSex
    {
        None,
        Male,
        Female
    }
}
