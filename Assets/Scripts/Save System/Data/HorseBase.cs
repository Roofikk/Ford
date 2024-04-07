using Ford.SaveSystem.Ver2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Ford.SaveSystem.Data
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
        public UserDate CreatedBy { get; set; }
        public UserDate LastModifiedBy { get; set; }
        public ICollection<HorseUserDto> Users { get; set; }
        public ICollection<SaveInfo> Saves { get; set; }

        [JsonIgnore]
        public string ActionDescription => $"Кличка: {Name}\nСоздан: {CreatedBy.Date}";

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
            CreatedBy = horse.CreatedBy;
            LastModifiedBy = horse.LastModifiedBy;
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
