using Ford.SaveSystem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Ford.WebApi.Data
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
        public ICollection<SaveInfo> Saves { get; set; } = new List<SaveInfo>();
        public ICollection<CreationHorseUser> Users { get; set; } = new List<CreationHorseUser>();
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
