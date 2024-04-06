using Ford.SaveSystem.Ver2;
using Ford.WebApi.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Ford.SaveSystem
{
    public class HorseBase : IStorageData
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
        public HorseUserDto Self { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public ICollection<HorseUserDto> Users { get; }
        public ICollection<SaveInfo> Saves { get; }

        public string ActionDescription => $"Кличка: {Name}\nДата:{CreationDate}";

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
            Self = horse.Self;
            Users = horse.Users;
            Saves = horse.Saves;
        }

        public HorseBase()
        {
            Users = new List<HorseUserDto>();
            Saves = new List<SaveInfo>();
        }
    }

    public enum HorseSex
    {
        None,
        Male,
        Female
    }
}
