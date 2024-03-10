using Ford.SaveSystem;
using System;
using System.Collections.Generic;

namespace Ford.WebApi.Data
{
    public class CreationHorse
    {
        public CreationHorse()
        {
            Saves = new List<SaveInfo>();
            Users = new List<CreationHorseUser>();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Sex { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string OwnerName { get; set; }
        public string OwnerPhoneNumber { get; set; }
        public ICollection<SaveInfo> Saves { get; set; }
        public ICollection<CreationHorseUser> Users { get; set; }
    }

    public class CreationHorseUser
    {
        public long UserId { get; set; }
        public bool IsOwner { get; set; }
        public string RuleAccess { get; set; }
    }

    public enum UserRoleAccess
    {
        Read,
        Write,
        All,
        Creator
    }
}
