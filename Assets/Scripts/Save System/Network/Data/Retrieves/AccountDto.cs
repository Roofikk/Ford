using Newtonsoft.Json;
using System;

namespace Ford.WebApi.Data
{
    public class AccountDto
    {
        public long UserId { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        [JsonConverter(typeof(DateConverter))]
        public DateTime? BirthDate { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
