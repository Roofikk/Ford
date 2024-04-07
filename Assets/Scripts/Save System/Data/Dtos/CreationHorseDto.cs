using Ford.SaveSystem.Data.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Ford.SaveSystem.Data
{
    public class CreationHorse
    {
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
        public List<CreatingSaveDto> Saves { get; set; } = new();
        public List<CreationHorseUser> Users { get; set; } = new();
    }

    public class CreationHorseUser
    {
        public long UserId { get; set; }
        public bool IsOwner { get; set; }
        public string AccessRole { get; set; }
    }

    public enum UserAccessRole
    {
        Read,
        Write,
        All,
        Creator
    }
}
