using Newtonsoft.Json;
using System;

namespace Ford.WebApi.Data
{
    public class UserDto
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        [JsonConverter(typeof(DateConverter))]
        public DateTime? BirthDate { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
